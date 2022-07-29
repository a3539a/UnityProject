/*
Copyright (c) 2021 Omar Duarte
Unauthorized copying of this file, via any medium is strictly prohibited.
Writen by Omar Duarte, 2021.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace PluginMaster
{
    #region DATA & SETTINGS 
    [Serializable]
    public class LineSettings : PaintOnSurfaceToolSettings, IPaintToolSettings
    {
        public enum SpacingType { BOUNDS, CONSTANT }

        [SerializeField] private Vector3 _projectionDirection = Vector3.down;
        [SerializeField] private bool _objectsOrientedAlongTheLine = true;
        [SerializeField] private AxesUtils.Axis _axisOrientedAlongTheLine = AxesUtils.Axis.X;
        [SerializeField] private SpacingType _spacingType = SpacingType.BOUNDS;
        [SerializeField] private float _gapSize = 0f;
        [SerializeField] private float _spacing = 10f;


        public Vector3 projectionDirection
        {
            get => _projectionDirection;
            set
            {
                if (_projectionDirection == value) return;
                _projectionDirection = value;
                OnDataChanged();
            }
        }
        public void UpdateProjectDirection(Vector3 value) => _projectionDirection = value;

        public bool objectsOrientedAlongTheLine
        {
            get => _objectsOrientedAlongTheLine;
            set
            {
                if (_objectsOrientedAlongTheLine == value) return;
                _objectsOrientedAlongTheLine = value;
                OnDataChanged();
            }
        }

        public AxesUtils.Axis axisOrientedAlongTheLine
        {
            get => _axisOrientedAlongTheLine;
            set
            {
                if (_axisOrientedAlongTheLine == value) return;
                _axisOrientedAlongTheLine = value;
                OnDataChanged();
            }
        }

        public SpacingType spacingType
        {
            get => _spacingType;
            set
            {
                if (_spacingType == value) return;
                _spacingType = value;
                OnDataChanged();
            }
        }

        public float spacing
        {
            get => _spacing;
            set
            {
                value = Mathf.Max(value, 0.01f);
                if (_spacing == value) return;
                _spacing = value;
                OnDataChanged();
            }
        }

        public float gapSize
        {
            get => _gapSize;
            set
            {
                if (_gapSize == value) return;
                _gapSize = value;
                OnDataChanged();
            }
        }

        [SerializeField] private PaintToolSettings _paintTool = new PaintToolSettings();
        public Transform parent { get => _paintTool.parent; set => _paintTool.parent = value; }
        public bool overwritePrefabLayer
        { get => _paintTool.overwritePrefabLayer; set => _paintTool.overwritePrefabLayer = value; }
        public int layer { get => _paintTool.layer; set => _paintTool.layer = value; }
        public bool autoCreateParent { get => _paintTool.autoCreateParent; set => _paintTool.autoCreateParent = value; }
        public bool createSubparentPerPalette
        {
            get => _paintTool.createSubparentPerPalette;
            set => _paintTool.createSubparentPerPalette = value;
        }
        public bool createSubparentPerTool
        {
            get => _paintTool.createSubparentPerTool;
            set => _paintTool.createSubparentPerTool = value;
        }
        public bool createSubparentPerBrush
        {
            get => _paintTool.createSubparentPerBrush;
            set => _paintTool.createSubparentPerBrush = value;
        }
        public bool createSubparentPerPrefab
        {
            get => _paintTool.createSubparentPerPrefab;
            set => _paintTool.createSubparentPerPrefab = value;
        }
        public bool overwriteBrushProperties
        { get => _paintTool.overwriteBrushProperties; set => _paintTool.overwriteBrushProperties = value; }
        public BrushSettings brushSettings => _paintTool.brushSettings;

        public LineSettings() : base()
        {
            OnDataChanged += DataChanged;
            _paintTool.OnDataChanged += DataChanged;
        }

        public override void DataChanged()
        {
            base.DataChanged();
            UpdateStroke();
            SceneView.RepaintAll();
        }

        protected virtual void UpdateStroke() => PWBIO.UpdateStroke();

        public override void Copy(IToolSettings other)
        {
            var otherLineSettings = other as LineSettings;
            if (otherLineSettings == null) return;
            base.Copy(other);
            _projectionDirection = otherLineSettings._projectionDirection;
            _objectsOrientedAlongTheLine = otherLineSettings._objectsOrientedAlongTheLine;
            _axisOrientedAlongTheLine = otherLineSettings._axisOrientedAlongTheLine;
            _spacingType = otherLineSettings._spacingType;
            _spacing = otherLineSettings._spacing;
            _paintTool.Copy(otherLineSettings._paintTool);
            _gapSize = otherLineSettings._gapSize;
        }

        public virtual LineSettings Clone()
        {
            var clone = new LineSettings();
            clone.Copy(this);
            return clone;
        }
    }

    [Serializable]
    public class LineSegment
    {
        public enum SegmentType { STRAIGHT, CURVE }
        public SegmentType type = SegmentType.CURVE;
        public List<Vector3> points = new List<Vector3>();
    }

    [Serializable]
    public class LinePoint
    {
        public LineSegment.SegmentType type = LineSegment.SegmentType.CURVE;
        public Vector3 position = Vector3.zero;
        public LinePoint(LineSegment.SegmentType type, Vector3 position) => (this.type, this.position) = (type, position);
        public LinePoint Clone() => new LinePoint(type, position);
    }

    [Serializable]
    public class LineData : ISerializationCallbackReceiver
    {
        public enum LineState
        {
            NONE,
            STRAIGHT_LINE,
            BEZIER
        }
        public const string COMMAND_NAME = "Edit Line";
        [SerializeField] private LineState _state = LineState.NONE;
        [SerializeField] private List<LinePoint> _controlPoints = new List<LinePoint>();
        [SerializeField] private int _selectedPointIdx = -1;
        [SerializeField] private List<int> _selection = new List<int>();
        [SerializeField] private bool _closed = false;
        private Vector3[] _points = null;
        private float _lenght = 0f;
        private List<Vector3> _midpoints = new List<Vector3>();
        private List<Vector3> _pathPoints = new List<Vector3>();
        public LineState state
        {
            get => _state;
            set
            {
                if (_state == value) return;
                ToolProperties.RegisterUndo(COMMAND_NAME);
                _state = value;
                if (_state == LineState.BEZIER) UpdatePath();
            }
        }
        public Vector3[] points => _points;
        public int pointsCount => _points.Length;
        public Vector3 GetPoint(int idx) => _points[idx];
        public Vector3 selectedPoint => _points[_selectedPointIdx];

        public void SetPoint(int idx, Vector3 value, bool registerUndo)
        {
            if (_points.Length <= 1) Initialize();
            if (_points[idx] == value) return;
            if (registerUndo) ToolProperties.RegisterUndo(COMMAND_NAME);
            var delta = value - _points[idx];
            _points[idx] = _controlPoints[idx].position = value;

            foreach (var selectedIdx in _selection)
            {
                if (selectedIdx == idx) continue;
                _controlPoints[selectedIdx].position += delta;
                _points[selectedIdx] = _controlPoints[selectedIdx].position;
            }
            UpdatePath();
        }

        public void RemoveSelectedPoints()
        {
            ToolProperties.RegisterUndo(COMMAND_NAME);
            var toRemove = new List<int>(_selection);
            if (!toRemove.Contains(_selectedPointIdx)) toRemove.Add(_selectedPointIdx);
            toRemove.Sort();
            if (toRemove.Count >= _points.Length - 1)
            {
                Initialize();
                return;
            }
            for (int i = toRemove.Count - 1; i >= 0; --i) _controlPoints.RemoveAt(toRemove[i]);
            _selectedPointIdx = -1;
            _selection.Clear();
            UpdatePoints();
        }

        public void InsertPoint(int idx, Vector3 point)
        {
            if (idx < 0) return;
            idx = Mathf.Max(idx, 1);
            ToolProperties.RegisterUndo(COMMAND_NAME);
            _controlPoints.Insert(idx, new LinePoint(_controlPoints[idx].type, point));
            UpdatePoints();
        }

        public int selectedPointIdx
        {
            get => _selectedPointIdx;
            set
            {
                if (_selectedPointIdx == value) return;
                ToolProperties.RegisterUndo(COMMAND_NAME);
                _selectedPointIdx = value;
            }
        }
        public void AddToSelection(int idx)
        {
            ToolProperties.RegisterUndo(COMMAND_NAME);
            if (!_selection.Contains(idx)) _selection.Add(idx);
        }
        public void SelectAll()
        {
            ToolProperties.RegisterUndo(COMMAND_NAME);
            _selection.Clear();
            for (int i = 0; i < pointsCount; ++i) _selection.Add(i);
            if (_selectedPointIdx < 0) _selectedPointIdx = 0;
        }
        public void RemoveFromSelection(int idx)
        {
            ToolProperties.RegisterUndo(COMMAND_NAME);
            if (_selection.Contains(idx)) _selection.Remove(idx);
        }
        public void ClearSelection()
        {
            ToolProperties.RegisterUndo(COMMAND_NAME);
            _selection.Clear();
        }
        public bool IsSelected(int idx) => _selection.Contains(idx);
        public int selectionCount => _selection.Count;
        public void SetSegmentType(LineSegment.SegmentType type)
        {
            ToolProperties.RegisterUndo(COMMAND_NAME);
            for (int i = 0; i < _selection.Count; ++i)
            {
                var idx = _selection[i];
                _controlPoints[idx].type = type;
            }
        }
        public LineSegment[] GetSegments()
        {
            var segments = new List<LineSegment>();

            var type = _controlPoints[0].type;
            for (int i = 0; i < pointsCount; ++i)
            {
                var segment = new LineSegment();
                segments.Add(segment);
                segment.type = type;
                segment.points.Add(_controlPoints[i].position);

                do
                {
                    ++i;
                    if (i >= pointsCount) break;
                    type = _controlPoints[i].type;
                    if (type == segment.type) segment.points.Add(_controlPoints[i].position);
                } while (type == segment.type);
                if (i >= pointsCount) break;
                i -= 2;
            }
            if (_closed)
            {
                if (_controlPoints[0].type == _controlPoints.Last().type)
                    segments.Last().points.Add(_controlPoints[0].position);
                else
                {
                    var segment = new LineSegment();
                    segment.type = _controlPoints[0].type;
                    segment.points.Add(_controlPoints.Last().position);
                    segment.points.Add(_controlPoints[0].position);
                    segments.Add(segment);
                }
            }
            return segments.ToArray();
        }
        public void Reset()
        {
            ToolProperties.RegisterUndo(COMMAND_NAME);
            Initialize();
        }

        private void UpdatePoints()
        {
            var pointsList = new List<Vector3>();
            foreach (var point in _controlPoints) pointsList.Add(point.position);
            _points = pointsList.ToArray();
            UpdatePath();
        }

        public void ToggleClosed()
        {
            ToolProperties.RegisterUndo(COMMAND_NAME);
            _closed = !_closed;
        }

        public bool closed => _closed;
        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        {
            UpdatePoints();
            PWBIO.repaint = true;
        }

        private void Initialize()
        {
            _selectedPointIdx = -1;
            _selection.Clear();
            _state = LineState.NONE;
            _controlPoints.Clear();
            for (int i = 0; i < 4; ++i) _controlPoints.Add(new LinePoint(LineSegment.SegmentType.CURVE, Vector3.zero));
            UpdatePoints();
        }
        private LineData() => Initialize();
        private static LineData _instance = null;
        public static LineData instance
        {
            get
            {
                if (_instance == null) _instance = new LineData();
                if (_instance.points == null || _instance.points.Length == 0) _instance.Initialize();
                return _instance;
            }
        }

        public LineData Clone()
        {
            var clone = new LineData();
            clone.Copy(this);
            clone._selectedPointIdx = -1;
            clone._selection.Clear();
            return clone;
        }

        public void Copy(LineData other)
        {
            _state = other._state;
            _controlPoints.Clear();
            foreach (var point in other._controlPoints) _controlPoints.Add(point.Clone());
            _selectedPointIdx = other._selectedPointIdx;
            _selection = other._selection.ToList();
            _closed = other._closed;
            _points = other._points == null ? null : other._points.ToArray();
            _lenght = other.lenght;
            _midpoints = other._midpoints.ToList();
            _pathPoints = other._pathPoints.ToList();
        }
        private float GetLineLength(Vector3[] points, out float[] lengthFromFirstPoint)
        {
            float lineLength = 0f;
            lengthFromFirstPoint = new float[points.Length];
            var segmentLength = new float[points.Length];
            lengthFromFirstPoint[0] = 0f;
            for (int i = 1; i < points.Length; ++i)
            {
                segmentLength[i - 1] = (points[i] - points[i - 1]).magnitude;
                lineLength += segmentLength[i - 1];
                lengthFromFirstPoint[i] = lineLength;
            }
            return lineLength;
        }

        private Vector3[] GetLineMidpoints(Vector3[] points)
        {
            if (points.Length == 0) return new Vector3[0];
            var midpoints = new List<Vector3>();
            var subSegments = new List<List<Vector3>>();
            var pathPoints = _points;
            bool IsAPathPoint(Vector3 point) => pathPoints.Contains(point);
            subSegments.Add(new List<Vector3>());
            subSegments.Last().Add(points[0]);
            for (int i = 1; i < points.Length - 1; ++i)
            {
                var point = points[i];
                subSegments.Last().Add(point);
                if (IsAPathPoint(point))
                {
                    subSegments.Add(new List<Vector3>());
                    subSegments.Last().Add(point);
                }
            }
            subSegments.Last().Add(points.Last());
            Vector3 GetLineMidpoint(Vector3[] subSegmentPoints)
            {
                var midpoint = subSegmentPoints[0];
                float[] lengthFromFirstPoint = null;
                var halfLineLength = GetLineLength(subSegmentPoints, out lengthFromFirstPoint) / 2f;
                for (int i = 1; i < subSegmentPoints.Length; ++i)
                {
                    if (lengthFromFirstPoint[i] < halfLineLength) continue;
                    var dir = (subSegmentPoints[i] - subSegmentPoints[i - 1]).normalized;
                    var localLength = halfLineLength - lengthFromFirstPoint[i - 1];
                    midpoint = subSegmentPoints[i - 1] + dir * localLength;
                    break;
                }
                return midpoint;
            }
            foreach (var subSegment in subSegments) midpoints.Add(GetLineMidpoint(subSegment.ToArray()));
            return midpoints.ToArray();
        }

        public void UpdatePath()
        {
            if (!LineManager.settings.editMode && state != LineState.BEZIER) return;
            _lenght = 0;
            _pathPoints.Clear();
            _midpoints.Clear();
            var segments = GetSegments();
            foreach (var segment in segments)
            {
                var segmentPoints = new List<Vector3>();
                if (segment.type == LineSegment.SegmentType.STRAIGHT) segmentPoints.AddRange(segment.points);
                else segmentPoints.AddRange(BezierPath.GetBezierPoints(segment.points.ToArray()));
                _pathPoints.AddRange(segmentPoints);
                if (segmentPoints.Count == 0) continue;
                _midpoints.AddRange(GetLineMidpoints(segmentPoints.ToArray()));
            }
            for (int i = 1; i < _pathPoints.Count; ++i)
                _lenght += (_pathPoints[i] - _pathPoints[i - 1]).magnitude;
        }

        public Vector3 NearestPathPoint(Vector3 point, float minPathLenght)
        {
            int pathIdx = -1;
            var p0 = _pathPoints[0];
            float lenght = 0;
            float minSqrDistance = float.MaxValue;
            for (int i = 1; i < _pathPoints.Count; ++i)
            {
                var p1 = _pathPoints[i];
                var delta = (p1 - p0).magnitude;
                lenght += delta;
                p0 = p1;
                if (lenght > minPathLenght)
                {
                    var sqrDistance = (point - p1).sqrMagnitude;
                    if (sqrDistance < minSqrDistance)
                    {
                        minSqrDistance = sqrDistance;
                        pathIdx = i;
                    }
                }
            }
            return pathIdx > 0 ? _pathPoints[pathIdx] : point;
        }
        public float lenght => _lenght;
        public Vector3[] pathPoints => _pathPoints.ToArray();
        public Vector3 lastPathPoint => _pathPoints.Last();
        public Vector3[] midpoints => _midpoints.ToArray();
    }

    [Serializable]
    public class ObjectId : IEquatable<ObjectId>
    {
        [SerializeField] private int _instanceId;
        [SerializeField] private string _globalObjId;

        public int instanceId { get => _instanceId; set => value = _instanceId; }
        public string globalObjId { get => _globalObjId; set => _globalObjId = value; }

        public ObjectId(GameObject gameObject)
        {
            if (gameObject == null)
            {
                _instanceId = -1;
                _globalObjId = null;
                return;
            }
            _instanceId = gameObject.GetInstanceID();
            _globalObjId = GlobalObjectId.GetGlobalObjectIdSlow(gameObject).ToString();
        }

        public ObjectId(int instanceId, string globalObjId)
        {
            _instanceId = instanceId;
            _globalObjId = globalObjId;
        }
        public override int GetHashCode()
        {
            int hashCode = 917907199;
            hashCode = hashCode * -1521134295 + _instanceId.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(_globalObjId);
            return hashCode;
        }
        public bool Equals(ObjectId other) => _instanceId == other._instanceId || _globalObjId == other._globalObjId;
        public override bool Equals(object obj) => obj is ObjectId other && this.Equals(other);
        public static bool operator ==(ObjectId lhs, ObjectId rhs) => lhs.Equals(rhs);
        public static bool operator !=(ObjectId lhs, ObjectId rhs) => !lhs.Equals(rhs);

        public void Copy(ObjectId other)
        {
            _instanceId = other._instanceId;
            _globalObjId = other._globalObjId;
        }

        public static GameObject FindObject(ObjectId objId)
        {
            var obj = EditorUtility.InstanceIDToObject(objId.instanceId) as GameObject;
            if (obj == null)
            {
                if (GlobalObjectId.TryParse(objId.globalObjId, out GlobalObjectId id))
                {
                    obj = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(id) as GameObject;
                    if (obj != null) objId.instanceId = obj.GetInstanceID();
                }
            }
            return obj;
        }
    }

    [Serializable]
    public class ObjectPose
    {
        [SerializeField] private ObjectId _id = null;
        [SerializeField] private Vector3 _position;
        [SerializeField] private Quaternion _localRotation;
        [SerializeField] private Vector3 _localScale;
        private GameObject _object = null;

        public ObjectPose(ObjectId id, Vector3 position, Quaternion localRotation, Vector3 localScale)
        {
            _id = id;
            _position = position;
            _localRotation = localRotation;
            _localScale = localScale;
            _object = ObjectId.FindObject(_id);
        }


        public ObjectId id
        {
            get => _id;
            set
            {
                if (_id == value) return;
                _id = value;
                _object = ObjectId.FindObject(_id);
            }
        }

        public GameObject obj
        {
            get
            {
                if (_object == null) _object = ObjectId.FindObject(_id);
                return _object;
            }
        }
        public Vector3 position { get => _position; set => _position = value; }
        public Quaternion localRotation { get => _localRotation; set => _localRotation = value; }
        public Vector3 localScale { get => _localScale; set => _localScale = value; }
        public ObjectPose Clone() => new ObjectPose(_id, _position, _localRotation, _localScale);
        public void Copy(ObjectPose other)
        {
            _position = other._position;
            _localRotation = other._localRotation;
            _localScale = other._localScale;
        }
    }

    [Serializable]
    public class PersistentLineData
    {
        private static long _nextId = -1;
        [SerializeField] private long _id = -1;
        [SerializeField] private LineData _data = null;
        [SerializeField] private LineSettings _settings = null;
        [SerializeField] private List<ObjectPose> _objectPoses = null;
        [SerializeField] private long _initialBrushId = -1;

        public static long nextId => _nextId;
        public static string HexId(long value) => "Line_" + value.ToString("X");
        public static string nextHexId => HexId(_nextId);

        public static void SetNextId() => _nextId = DateTime.Now.Ticks;

        private PersistentLineData() { }
        public PersistentLineData(LineData data, LineSettings settings, GameObject[] objects, long initialBrushId)
        {
            _id = _nextId;
            SetNextId();
            _data = data.Clone();
            _settings = settings.Clone();
            if (objects == null || objects.Length == 0) return;
            _objectPoses = new List<ObjectPose>();
            AddObjects(objects);
            _initialBrushId = initialBrushId;
        }

        public long id => _id;
        public string hexId => HexId(_id);
        public LineData data => _data;
        public LineSettings settings => _settings;
        public long initialBrushId => _initialBrushId;

        public Vector3 lastTangentPos { get; set; }
        public PersistentLineData Clone()
        {
            var clone = new PersistentLineData();
            clone._id = _id;
            clone._data = _data.Clone();
            clone._settings = _settings.Clone();
            clone._objectPoses = new List<ObjectPose>();
            foreach (var objPos in _objectPoses) clone._objectPoses.Add(objPos.Clone());
            clone._initialBrushId = _initialBrushId;
            return clone;
        }

        public void UpdateObjects()
        {
            if (_objectPoses == null) return;
            var objPos = _objectPoses.ToArray();
            foreach (var item in objPos)
            {
                var obj = item.obj;
                if (obj == null) _objectPoses.Remove(item);
            }
        }

        public void UpdatePoses()
        {
            var objPos = _objectPoses.ToArray();
            foreach (var item in objPos)
            {
                var obj = item.obj;
                if (obj == null)
                {
                    _objectPoses.Remove(item);
                    continue;
                }
                item.position = obj.transform.position;
                item.localRotation = obj.transform.localRotation;
                item.localScale = obj.transform.localScale;
            }
        }

        public void AddObjects(GameObject[] objects)
        {
            for (int i = 0; i < objects.Length; ++i)
                _objectPoses.Add(new ObjectPose(new ObjectId(objects[i]),
                    objects[i].transform.position, objects[i].transform.localRotation, objects[i].transform.localScale));
        }

        public int objectCount => _objectPoses == null ? 0 : _objectPoses.Count;
        public ObjectPose[] objectPoses => _objectPoses.ToArray();
        public GameObject[] objects
        {
            get
            {
                var objs = new List<GameObject>();
                foreach (var item in _objectPoses) objs.Add(item.obj);
                return objs.ToArray();
            }
        }

        public List<GameObject> objectList
        {
            get
            {
                var list = new List<GameObject>();
                var objPos = _objectPoses.ToArray();
                foreach (var item in objPos)
                {
                    var obj = item.obj;
                    if (obj == null)
                    {
                        _objectPoses.Remove(item);
                        continue;
                    }
                    list.Add(obj);
                }
                return list;
            }
        }

        public void ResetPoses(PersistentLineData initialData)
        {
            var initialPoses = initialData.objectPoses;
            foreach (var initialPose in initialPoses)
            {
                var pose = _objectPoses.Find(p => p.id == initialPose.id);
                if (pose == null) continue;
                pose.Copy(initialPose);
                if (pose.obj == null) continue;
                pose.obj.transform.position = pose.position;
                pose.obj.transform.localRotation = pose.localRotation;
                pose.obj.transform.localScale = pose.localScale;
                pose.obj.SetActive(true);
            }
            _data.Copy(initialData.data);
        }
    }

    [Serializable]
    public class SceneLineData
    {
        [SerializeField] private string _sceneGUID = null;
        [SerializeField] private List<PersistentLineData> _lines = null;

        public string sceneGUID => _sceneGUID;
        public List<PersistentLineData> lines => _lines;

        public SceneLineData(string sceneGUID)
            => _sceneGUID = sceneGUID;

        public void AddLine(PersistentLineData data)
        {
            if (_lines == null) _lines = new List<PersistentLineData>();
            _lines.Add(data);
        }

        public void RemoveLineData(long lineId)
            => _lines.RemoveAll(l => l.id == lineId);

        public PersistentLineData GetLine(long lineId) => _lines.Find(l => l.id == lineId);
    }

    [Serializable]
    public class PersistentLineSettings : LineSettings
    {
        [SerializeField] private List<SceneLineData> _sceneLines = null;
        private bool _editMode = false;
        public Action OnToolModeChanged;
        public bool editMode
        {
            get => _editMode;
            set
            {
                if (_editMode == value) return;
                _editMode = value;
                if (OnToolModeChanged != null) OnToolModeChanged();
            }
        }
        public void AddPersistentLine(string sceneGUID, PersistentLineData data)
        {
            if (_sceneLines == null) _sceneLines = new List<SceneLineData>();
            var item = _sceneLines.Find(i => i.sceneGUID == sceneGUID);
            if (item == null)
            {
                item = new SceneLineData(sceneGUID);
                _sceneLines.Add(item);
            }
            if (item.lines != null)
            {
                var line = item.lines.Find(l => l.id == data.id);
                if (line != null) return;
            }
            item.AddLine(data);
            PWBCore.staticData.Save();
        }

        public PersistentLineData[] GetPersistentLines()
        {
            var lines = new List<PersistentLineData>();
            var openedSceneCount = EditorSceneManager.sceneCount;
            if (_sceneLines != null)
            {
                for (int i = 0; i < openedSceneCount; ++i)
                {
                    string sceneGUID = AssetDatabase.AssetPathToGUID(SceneManager.GetSceneAt(i).path);
                    var data = _sceneLines.Find(item => item.sceneGUID == sceneGUID);
                    if (data == null)
                    {
                        _sceneLines.Remove(data);
                        continue;
                    }
                    lines.AddRange(data.lines);
                }
            }
            return lines.ToArray();
        }


        public void RemovePersistentLine(long lineId)
        {
            foreach (var item in _sceneLines) item.RemoveLineData(lineId);
            PWBCore.staticData.Save();
        }

        public PersistentLineData GetLine(long lineId)
        {
            var lines = GetPersistentLines();
            foreach (var line in lines)
                if (line.id == lineId) return line;
            return null;
        }
    }

    [Serializable]
    public class LineManager : ToolManagerBase<PersistentLineSettings> { }
    #endregion

    #region PWBIO
    public static partial class PWBIO
    {
        private static LineData _lineData = LineData.instance;
        private static bool _selectingLinePoints = false;
        private static Rect _selectionRect = new Rect();
        private static List<GameObject> _objectsOutOfTheLine = null;
        private static bool _edittingPersistentLine = false;
        private static PersistentLineData _initialPersistentLineData = null;
        private static PersistentLineData _selectedPersistentLineData = null;
        private static string _createProfileNane = ToolProfile.DEFAULT;
        private static void LineInitializeOnLoad()
        {
            LineManager.settings.OnToolModeChanged += OnLineToolModeChanged;
            LineManager.settings.OnDataChanged += OnLineSettingsChanged;
        }
        public static void ResetLineState(bool askIfWantToSave = true)
        {
            if (askIfWantToSave)
            {
                void Save()
                {
                    if (SceneView.lastActiveSceneView != null)
                        LineStrokePreview(SceneView.lastActiveSceneView.camera, LineManager.settings, _lineData.lastPathPoint);
                    CreateLine();
                }
                AskIfWantToSave((int)_lineData.state, (int)LineData.LineState.BEZIER, Save);
            }
            _snappedToVertex = false;
            _selectingLinePoints = false;
            _lineData.Reset();
        }

        private static void DeselectPersistentLines()
        {
            var persistentLines = LineManager.settings.GetPersistentLines();
            foreach (var l in persistentLines)
            {
                l.data.selectedPointIdx = -1;
                l.data.ClearSelection();
            }
        }

        private static void OnLineToolModeChanged()
        {
            DeselectPersistentLines();
            if (!LineManager.settings.editMode)
            {
                if (_createProfileNane != null)
                    ToolProperties.SetProfile(new ToolProperties.ProfileData(LineManager.instance, _createProfileNane));
                ToolProperties.RepainWindow();
                return;
            }
            ResetLineState();
            ResetSelectedPersistentLine();
        }

        private static void OnLineSettingsChanged()
        {
            if (!LineManager.settings.editMode) return;
            if (_selectedPersistentLineData == null) return;
            _selectedPersistentLineData.settings.Copy(LineManager.settings);
            PreviewPersistenLine(_selectedPersistentLineData);
        }

        private static void OnLineUndo()
        {
            _paintStroke.Clear();
            BrushstrokeManager.ClearBrushstroke();
        }

        private static void LineDuringSceneGUI(SceneView sceneView)
        {
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
            {
                if (_lineData.state == LineData.LineState.BEZIER && _lineData.selectedPointIdx > 0)
                {
                    _lineData.selectedPointIdx = -1;
                    _lineData.ClearSelection();
                }
                else if (_lineData.state == LineData.LineState.NONE && !LineManager.settings.editMode)
                    ToolManager.DeselectTool();
                else if (LineManager.settings.editMode)
                {
                    if (_edittingPersistentLine) ResetSelectedPersistentLine();
                    else
                    {
                        ToolManager.DeselectTool();
                        DeselectPersistentLines();
                    }
                    _initialPersistentLineData = null;
                    _selectedPersistentLineData = null;
                    ToolProperties.SetProfile(new ToolProperties.ProfileData(LineManager.instance, _createProfileNane));
                    ToolProperties.RepainWindow();
                    LineManager.settings.editMode = false;
                }
                else ResetLineState();
                _paintStroke.Clear();
                OnLineUndo();
                UpdateStroke();
                BrushstrokeManager.ClearBrushstroke();
            }
            LineToolEditMode(sceneView);
            DrawSelectionRectangle();
            if (LineManager.settings.editMode)
            {
                return;
            }
            switch (_lineData.state)
            {
                case LineData.LineState.NONE:
                    LineStateNone(sceneView.in2DMode);
                    break;
                case LineData.LineState.STRAIGHT_LINE:
                    LineStateStraightLine(sceneView.in2DMode);
                    break;
                case LineData.LineState.BEZIER:
                    LineStateBezier(sceneView);
                    break;
            }
        }

        private static bool DrawLineControlPoints(LineData lineData, SceneView sceneView,
            out bool clickOnPoint, out bool wasEdited, bool showHandles, out Vector3 delta)
        {
            delta = Vector3.zero;
            clickOnPoint = false;
            wasEdited = false;
            bool leftMouseDown = Event.current.button == 0 && Event.current.type == EventType.MouseDown;
            for (int i = 0; i < lineData.pointsCount; ++i)
            {
                var controlId = GUIUtility.GetControlID(FocusType.Passive);
                if (_selectingLinePoints)
                {
                    var GUIPos = HandleUtility.WorldToGUIPoint(lineData.GetPoint(i));
                    var rect = _selectionRect;
                    if (_selectionRect.size.x < 0 || _selectionRect.size.y < 0)
                    {
                        var max = Vector2.Max(_selectionRect.min, _selectionRect.max);
                        var min = Vector2.Min(_selectionRect.min, _selectionRect.max);
                        var size = max - min;
                        rect = new Rect(min, size);
                    }
                    if (rect.Contains(GUIPos))
                    {
                        if (!Event.current.control && lineData.selectedPointIdx < 0)
                            lineData.selectedPointIdx = i;
                        lineData.AddToSelection(i);
                    }
                }
                else if (!clickOnPoint)
                {
                    if (showHandles)
                    {
                        float distFromMouse
                            = HandleUtility.DistanceToRectangle(lineData.GetPoint(i), Quaternion.identity, 0f);
                        HandleUtility.AddControl(controlId, distFromMouse);
                        if (leftMouseDown && HandleUtility.nearestControl == controlId)
                        {
                            if (!Event.current.control)
                            {
                                lineData.selectedPointIdx = i;
                                lineData.ClearSelection();
                            }
                            if (Event.current.control || lineData.selectionCount == 0)
                            {
                                if (lineData.IsSelected(i)) lineData.RemoveFromSelection(i);
                                else lineData.AddToSelection(i);
                            }
                            clickOnPoint = true;
                            Event.current.Use();
                        }
                    }
                }
                if (Event.current.type != EventType.Repaint) continue;
                DrawDotHandleCap(lineData.GetPoint(i), 1, 1, lineData.IsSelected(i));
            }
            var midpoints = lineData.midpoints;
            for (int i = 0; i < midpoints.Length; ++i)
            {
                var point = midpoints[i];

                var controlId = GUIUtility.GetControlID(FocusType.Passive);
                if (showHandles)
                {
                    float distFromMouse
                           = HandleUtility.DistanceToRectangle(point, Quaternion.identity, 0f);
                    HandleUtility.AddControl(controlId, distFromMouse);
                }
                DrawDotHandleCap(point, 0.4f);
                if (showHandles && HandleUtility.nearestControl == controlId)
                {
                    DrawDotHandleCap(point);
                    if (leftMouseDown)
                    {
                        lineData.InsertPoint(i + 1, point);
                        lineData.selectedPointIdx = i + 1;
                        lineData.ClearSelection();
                        _updateStroke = true;
                        clickOnPoint = true;
                        Event.current.Use();
                    }
                }
            }
            if (showHandles && lineData.selectedPointIdx >= 0)
            {
                var prevPosition = lineData.selectedPoint;
                lineData.SetPoint(lineData.selectedPointIdx,
                    Handles.PositionHandle(lineData.selectedPoint, Quaternion.identity), true);
                var point = _snapToVertex ? LinePointSnapping(lineData.selectedPoint)
                    : SnapAndUpdateGridOrigin(lineData.selectedPoint, SnapManager.settings.snappingEnabled,
                        LineManager.settings.paintOnPalettePrefabs, LineManager.settings.paintOnMeshesWithoutCollider, false);
                lineData.SetPoint(lineData.selectedPointIdx, point, true);
                if (prevPosition != lineData.selectedPoint)
                {
                    wasEdited = true;
                    delta = lineData.selectedPoint - prevPosition;
                }
            }
            if (!showHandles) return false;
            return clickOnPoint || wasEdited;
        }

        private static Vector3 LinePointSnapping(Vector3 point)
        {
            const float snapSqrDistance = 400f;
            var mouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            var persistentLines = LineManager.settings.GetPersistentLines();
            var result = point;
            var minSqrDistance = snapSqrDistance;
            foreach (var lineData in persistentLines)
            {
                var controlPoints = lineData.data.points;
                foreach (var controlPoint in controlPoints)
                {
                    var intersection = mouseRay.origin + Vector3.Project(controlPoint - mouseRay.origin, mouseRay.direction);
                    var GUIControlPoint = HandleUtility.WorldToGUIPoint(controlPoint);
                    var intersectionGUIPoint = HandleUtility.WorldToGUIPoint(intersection);
                    var sqrDistance = (GUIControlPoint - intersectionGUIPoint).sqrMagnitude;
                    if (sqrDistance > 0 && sqrDistance < snapSqrDistance && sqrDistance < minSqrDistance)
                    {
                        minSqrDistance = sqrDistance;
                        result = controlPoint;
                    }
                }
            }
            return result;
        }

        private static void LineToolEditMode(SceneView sceneView)
        {

            if (_selectedPersistentLineData != null
                && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Delete)
            {
                _selectedPersistentLineData.data.RemoveSelectedPoints();
                PreviewPersistenLine(_selectedPersistentLineData);
            }
            var persistentLines = LineManager.settings.GetPersistentLines();
            var selectedLineId = _initialPersistentLineData == null ? -1 : _initialPersistentLineData.id;
            bool clickOnAnyPoint = false;

            foreach (var lineData in persistentLines)
            {
                lineData.UpdateObjects();
                if (lineData.objectCount == 0)
                {
                    LineManager.settings.RemovePersistentLine(lineData.id);
                    continue;
                }
                DrawLine(lineData.data);

                if (DrawLineControlPoints(lineData.data, sceneView,
                    out bool clickOnPoint, out bool wasEdited, LineManager.settings.editMode, out Vector3 delta))
                {
                    if (clickOnPoint)
                    {
                        clickOnAnyPoint = true;
                        _edittingPersistentLine = true;
                        if (selectedLineId != lineData.id)
                        {
                            ApplySelectedPersistentLine(false);
                            if (selectedLineId == -1)
                                _createProfileNane = LineManager.instance.selectedProfileName;
                            LineManager.instance.CopyToolSettings(lineData.settings);
                            ToolProperties.RepainWindow();
                        }
                        _selectedPersistentLineData = lineData;
                        if (_initialPersistentLineData == null) _initialPersistentLineData = lineData.Clone();
                        else if (_initialPersistentLineData.id != lineData.id) _initialPersistentLineData = lineData.Clone();

                        foreach (var l in persistentLines)
                        {
                            if (l == lineData) continue;
                            l.data.selectedPointIdx = -1;
                            l.data.ClearSelection();
                        }
                    }
                    if (wasEdited)
                    {
                        _edittingPersistentLine = true;
                        PreviewPersistenLine(lineData);
                    }
                }
            }

            if (!LineManager.settings.editMode) return;

            SelectionRectangleInput(clickOnAnyPoint);

            if (_edittingPersistentLine && _selectedPersistentLineData != null)
            {
                if (_updateStroke)
                {
                    _selectedPersistentLineData.data.UpdatePath();
                    PreviewPersistenLine(_selectedPersistentLineData);
                    _updateStroke = false;
                }
                LineStrokePreview(sceneView.camera, _selectedPersistentLineData.settings,
                    _selectedPersistentLineData.data.lastPathPoint, true,
                    _selectedPersistentLineData.objectCount, _selectedPersistentLineData.lastTangentPos);
            }

            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
            {
                void DeleteObjectsOutTheLine()
                {
                    if (_objectsOutOfTheLine == null) return;
                    foreach (var obj in _objectsOutOfTheLine)
                    {
                        if (obj == null) continue;
                        obj.SetActive(true);
                        Undo.DestroyObjectImmediate(obj);
                    }
                }
                DeleteObjectsOutTheLine();
                ApplySelectedPersistentLine(true);
                ToolProperties.SetProfile(new ToolProperties.ProfileData(LineManager.instance, _createProfileNane));
                DeleteObjectsOutTheLine();
                ToolProperties.RepainWindow();
            }
        }

        private static void PreviewPersistenLine(PersistentLineData lineData)
        {
            Vector3[] objPos = null;
            var objList = lineData.objectList;
            Vector3[] strokePos = null;
            BrushstrokeManager.UpdatePersistentLineBrushstroke(lineData.data.pathPoints,
                lineData.settings, objList, out objPos, out strokePos);
            _objectsOutOfTheLine = lineData.objects.ToList();
            float pathLength = 0;

            for (int objIdx = 0; objIdx < objPos.Length; ++objIdx)
            {
                var obj = objList[objIdx];

                obj.SetActive(true);
                if (objIdx > 0) pathLength += (objPos[objIdx] - objPos[objIdx - 1]).magnitude;

                var bounds = BoundsUtils.GetBoundsRecursive(obj.transform, obj.transform.rotation);
                var size = bounds.size;
                var pivotToCenter = bounds.center - obj.transform.position;
                var pivotToCenterLocal = obj.transform.InverseTransformVector(pivotToCenter);
                var height = Mathf.Max(size.x, size.y, size.z) * 2;
                Vector3 segmentDir = Vector3.zero;
                var objOnLineSize = AxesUtils.GetAxisValue(size, lineData.settings.axisOrientedAlongTheLine);
                if (lineData.settings.objectsOrientedAlongTheLine && objPos.Length > 1)
                {
                    if (objIdx < objPos.Length - 1)
                    {
                        if (objIdx + 1 < objPos.Length) segmentDir = objPos[objIdx + 1] - objPos[objIdx];
                        else if (strokePos.Length > 0) segmentDir = strokePos[0] - objPos[objIdx];
                    }
                    else
                    {
                        var pivotTocenterOnLine = AxesUtils.GetAxisValue(pivotToCenterLocal,
                        lineData.settings.axisOrientedAlongTheLine);
                        var pivotToEndSize = objOnLineSize / 2 + pivotTocenterOnLine;
                        var nearestPathPoint = lineData.data.NearestPathPoint(objPos[objIdx], pathLength + pivotToEndSize);
                        segmentDir = nearestPathPoint - objPos[objIdx];
                    }
                }
                if (objPos.Length == 1) segmentDir = lineData.data.lastPathPoint - objPos[0];
                else if (objIdx == objPos.Length - 1)
                {
                    var onLineSize = objOnLineSize + lineData.settings.gapSize;
                    var segmentSize = segmentDir.magnitude;
                    if (segmentSize > onLineSize) segmentDir = segmentDir.normalized * (onLineSize);
                }
                var normal = -lineData.settings.projectionDirection;
                var otherAxes = AxesUtils.GetOtherAxes((AxesUtils.SignedAxis)(-lineData.settings.projectionDirection));
                var tangetAxis = otherAxes[lineData.settings.objectsOrientedAlongTheLine ? 0 : 1];
                Vector3 itemTangent = (AxesUtils.SignedAxis)(tangetAxis);
                var itemRotation = Quaternion.LookRotation(itemTangent, normal);
                var lookAt = Quaternion.LookRotation((Vector3)(AxesUtils.SignedAxis)
                    (lineData.settings.axisOrientedAlongTheLine), Vector3.up);
                if (segmentDir != Vector3.zero) itemRotation = Quaternion.LookRotation(segmentDir, normal) * lookAt;
                var pivotPosition = objPos[objIdx]
                    - segmentDir.normalized * Mathf.Abs(AxesUtils.GetAxisValue(pivotToCenterLocal, tangetAxis));
                var itemPosition = pivotPosition + segmentDir / 2;
                var ray = new Ray(itemPosition + normal * height, -normal);
                if (lineData.settings.mode != LineSettings.PaintMode.ON_SHAPE)
                {
                    if (MouseRaycast(ray, out RaycastHit itemHit, out GameObject collider, height * 2f, -1,
                        lineData.settings.paintOnPalettePrefabs, lineData.settings.paintOnMeshesWithoutCollider))
                    {
                        itemPosition = itemHit.point;
                        normal = itemHit.normal;
                    }
                    else if (lineData.settings.mode == LineSettings.PaintMode.ON_SURFACE) continue;
                }
                BrushSettings brushSettings = new BrushSettings();
                if (lineData.settings.overwriteBrushProperties) brushSettings = lineData.settings.brushSettings;
                if (brushSettings.rotateToTheSurface && segmentDir != Vector3.zero)
                    itemRotation = Quaternion.LookRotation(segmentDir, normal) * lookAt;
                itemPosition += normal * brushSettings.surfaceDistance;
                if (brushSettings.embedInSurface && lineData.settings.mode != LineSettings.PaintMode.ON_SHAPE)
                {
                    var TRS = Matrix4x4.TRS(itemPosition, itemRotation, Vector3.one);
                    var bottomVertices = BoundsUtils.GetBottomVertices(obj.transform);
                    var bottomMagnitude = BoundsUtils.GetBottomMagnitude(obj.transform);
                    var bottomDistanceToSurfce = GetBottomDistanceToSurface(bottomVertices, TRS, height,
                        lineData.settings.paintOnPalettePrefabs, lineData.settings.paintOnMeshesWithoutCollider);
                    if (!brushSettings.embedAtPivotHeight) bottomDistanceToSurfce -= bottomMagnitude;
                    itemPosition += itemRotation * new Vector3(0f, -bottomDistanceToSurfce, 0f);
                }
                itemPosition += itemRotation * brushSettings.localPositionOffset;
                Undo.RecordObject(obj.transform, LineData.COMMAND_NAME);
                obj.transform.position = itemPosition;
                obj.transform.rotation = itemRotation;
                _objectsOutOfTheLine.Remove(obj);
                lineData.lastTangentPos = objPos[objIdx];
            }
            foreach (var obj in _objectsOutOfTheLine) obj.SetActive(false);
        }

        private static void ResetSelectedPersistentLine()
        {
            _edittingPersistentLine = false;
            if (_initialPersistentLineData == null) return;
            var selectedLine = LineManager.settings.GetLine(_initialPersistentLineData.id);
            if (selectedLine == null) return;
            selectedLine.ResetPoses(_initialPersistentLineData);
            selectedLine.data.selectedPointIdx = -1;
            selectedLine.data.ClearSelection();
        }

        private static void ApplySelectedPersistentLine(bool deselectPoint)
        {
            _edittingPersistentLine = false;
            if (_initialPersistentLineData == null) return;
            var selectedLine = LineManager.settings.GetLine(_initialPersistentLineData.id);
            if (selectedLine == null)
            {
                _initialPersistentLineData = null;
                _selectedPersistentLineData = null;
                return;
            }
            selectedLine.UpdatePoses();
            if (_paintStroke.Count > 0)
            {
                var objs = Paint(LineManager.settings, PAINT_CMD, true, selectedLine.hexId);
                selectedLine.AddObjects(objs);
            }

            _initialPersistentLineData = selectedLine.Clone();
            var persistentLines = LineManager.settings.GetPersistentLines();
            if (!deselectPoint) return;
            foreach (var line in persistentLines)
            {
                line.data.selectedPointIdx = -1;
                line.data.ClearSelection();
            }
        }
        private static void LineStateNone(bool in2DMode)
        {
            if (Event.current.button == 0 && Event.current.type == EventType.MouseDown && !Event.current.alt)
            {
                _lineData.state = LineData.LineState.STRAIGHT_LINE;
                Event.current.Use();
            }
            if (MouseDot(out Vector3 point, out Vector3 normal, LineManager.settings.mode, in2DMode,
                LineManager.settings.paintOnPalettePrefabs, LineManager.settings.paintOnMeshesWithoutCollider, false))
            {
                point = _snapToVertex ? LinePointSnapping(point)
                    : SnapAndUpdateGridOrigin(point, SnapManager.settings.snappingEnabled,
                    LineManager.settings.paintOnPalettePrefabs, LineManager.settings.paintOnMeshesWithoutCollider,
                    false);
                _lineData.SetPoint(0, point, false);
                _lineData.SetPoint(3, point, false);
            }
            DrawDotHandleCap(_lineData.GetPoint(0));
        }

        private static void LineStateStraightLine(bool in2DMode)
        {
            if (Event.current.button == 0 && Event.current.type == EventType.MouseDown && !Event.current.alt)
            {
                var lineThird = (_lineData.GetPoint(3) - _lineData.GetPoint(0)) / 3f;
                _lineData.SetPoint(1, _lineData.GetPoint(0) + lineThird, false);
                _lineData.SetPoint(2, _lineData.GetPoint(1) + lineThird, false);
                _lineData.state = LineData.LineState.BEZIER;
                _updateStroke = true;
            }
            if (MouseDot(out Vector3 point, out Vector3 normal, LineManager.settings.mode, in2DMode,
                LineManager.settings.paintOnPalettePrefabs, LineManager.settings.paintOnMeshesWithoutCollider, false))
            {
                point = _snapToVertex ? LinePointSnapping(point)
                    : SnapAndUpdateGridOrigin(point, SnapManager.settings.snappingEnabled,
                    LineManager.settings.paintOnPalettePrefabs, LineManager.settings.paintOnMeshesWithoutCollider,
                    false);
                _lineData.SetPoint(3, point, false);
            }

            Handles.color = new Color(0f, 0f, 0f, 0.7f);
            Handles.DrawAAPolyLine(8, new Vector3[] { _lineData.GetPoint(0), _lineData.GetPoint(3) });
            Handles.color = new Color(1f, 1f, 1f, 0.7f);
            Handles.DrawAAPolyLine(4, new Vector3[] { _lineData.GetPoint(0), _lineData.GetPoint(3) });
            DrawDotHandleCap(_lineData.GetPoint(0));
            DrawDotHandleCap(_lineData.GetPoint(3));
        }

        private static void DrawLine(LineData lineData)
        {
            var pathPoints = lineData.pathPoints;
            Handles.zTest = CompareFunction.Always;
            Handles.color = new Color(0f, 0f, 0f, 0.7f);
            Handles.DrawAAPolyLine(8, pathPoints);
            Handles.color = new Color(1f, 1f, 1f, 0.7f);
            Handles.DrawAAPolyLine(4, pathPoints);
        }

        private static void DrawSelectionRectangle()
        {
            if (!_selectingLinePoints) return;
            var rays = new Ray[]
            {
                HandleUtility.GUIPointToWorldRay(_selectionRect.min),
                HandleUtility.GUIPointToWorldRay(new Vector2(_selectionRect.xMax, _selectionRect.yMin)),
                HandleUtility.GUIPointToWorldRay(_selectionRect.max),
                HandleUtility.GUIPointToWorldRay(new Vector2(_selectionRect.xMin, _selectionRect.yMax))
            };
            var verts = new Vector3[4];
            for (int i = 0; i < 4; ++i) verts[i] = rays[i].origin + rays[i].direction;
            Handles.DrawSolidRectangleWithOutline(verts,
            new Color(0f, 0.5f, 0.5f, 0.3f), new Color(0f, 0.5f, 0.5f, 1f));
        }

        private static void SelectionRectangleInput(bool clickOnPoint)
        {
            bool leftMouseDown = Event.current.button == 0 && Event.current.type == EventType.MouseDown;
            if (Event.current.shift && leftMouseDown && !clickOnPoint)
            {
                _selectingLinePoints = true;
                _selectionRect = new Rect(Event.current.mousePosition, Vector2.zero);
            }
            else if (Event.current.type == EventType.MouseDrag && _selectingLinePoints)
            {
                _selectionRect.size = Event.current.mousePosition - _selectionRect.position;
            }
            else if (_selectingLinePoints && Event.current.button == 0
                && (Event.current.type == EventType.MouseUp || Event.current.type == EventType.Ignore))
                _selectingLinePoints = false;
        }

        private static void CreateLine()
        {
            var objs = Paint(LineManager.settings, PAINT_CMD, true, PersistentLineData.nextHexId);
            var scenePath = EditorSceneManager.GetActiveScene().path;
            var sceneGUID = AssetDatabase.AssetPathToGUID(scenePath);
            var initialBrushId = PaletteManager.selectedBrush != null ? PaletteManager.selectedBrush.id : -1;
            var persistentData = new PersistentLineData(_lineData, LineManager.settings, objs, initialBrushId);
            LineManager.settings.AddPersistentLine(sceneGUID, persistentData);
        }

        private static void LineStateBezier(SceneView sceneView)
        {
            var pathPoints = _lineData.pathPoints;
            if (_updateStroke)
            {
                _lineData.UpdatePath();
                pathPoints = _lineData.pathPoints;
                BrushstrokeManager.UpdateLineBrushstroke(pathPoints);
                SceneView.RepaintAll();
                _updateStroke = false;
            }
            LineStrokePreview(sceneView.camera, LineManager.settings, _lineData.lastPathPoint);
            DrawLine(_lineData);
            DrawSelectionRectangle();
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
            {
                CreateLine();
                ResetLineState(false);
            }
            else if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Delete)
            {
                _lineData.RemoveSelectedPoints();
                _updateStroke = true;
            }
            else if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.A
                && Event.current.control && Event.current.shift)
            {
                _lineData.SelectAll();
            }
            else if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.D
                && Event.current.control && Event.current.shift)
            {
                _lineData.selectedPointIdx = -1;
                _lineData.ClearSelection();
            }
            else if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.PageUp
                || (Event.current.control && Event.current.keyCode == KeyCode.UpArrow)))
            {
                _lineData.SetSegmentType(LineSegment.SegmentType.CURVE);
                _updateStroke = true;
            }
            else if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.PageDown
                || (Event.current.control && Event.current.keyCode == KeyCode.DownArrow)))
            {
                _lineData.SetSegmentType(LineSegment.SegmentType.STRAIGHT);
                _updateStroke = true;
            }
            else if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.End
                || (Event.current.control && Event.current.shift && Event.current.keyCode == KeyCode.O)))
            {
                _lineData.ToggleClosed();
                _updateStroke = true;
            }
            else if (Event.current.button == 1 && Event.current.type == EventType.MouseDrag
                && Event.current.shift && Event.current.control)
            {
                var deltaSign = Mathf.Sign(Event.current.delta.x + Event.current.delta.y);
                LineManager.settings.gapSize += _lineData.lenght * deltaSign * 0.001f;
                ToolProperties.RepainWindow();
                Event.current.Use();
            }

            if (_selectingLinePoints && !Event.current.control)
            {
                _lineData.selectedPointIdx = -1;
                _lineData.ClearSelection();
            }
            bool clickOnPoint, wasEdited;
            DrawLineControlPoints(_lineData, sceneView, out clickOnPoint, out wasEdited, true, out Vector3 delta);
            if (wasEdited) _updateStroke = true;
            SelectionRectangleInput(clickOnPoint);
        }

        private static void LineStrokePreview(Camera camera, LineSettings settings,
            Vector3 lastPoint, bool persistent = false,
            int objectCount = 0, Vector3 lastObjectTangentPosition = new Vector3())
        {
            _paintStroke.Clear();
            for (int i = 0; i < BrushstrokeManager.brushstroke.Length; ++i)
            {
                var strokeItem = BrushstrokeManager.brushstroke[i];
                var prefab = strokeItem.settings.prefab;
                if (prefab == null) continue;
                var bounds = BoundsUtils.GetBoundsRecursive(prefab.transform);
                var size = bounds.size;
                var pivotToCenter = bounds.center - prefab.transform.position;
                var pivotToCenterLocal = prefab.transform.InverseTransformVector(pivotToCenter);
                var height = Mathf.Max(size.x, size.y, size.z) * 2;
                Vector3 segmentDir = Vector3.zero;

                if (settings.objectsOrientedAlongTheLine && BrushstrokeManager.brushstroke.Length > 1)
                {
                    segmentDir = i < BrushstrokeManager.brushstroke.Length - 1
                        ? strokeItem.nextTangentPosition - strokeItem.tangentPosition
                        : lastPoint - strokeItem.tangentPosition;
                }
                if (BrushstrokeManager.brushstroke.Length == 1)
                {
                    segmentDir = lastPoint - BrushstrokeManager.brushstroke[0].tangentPosition;
                    if (persistent && objectCount > 0)
                        segmentDir = lastPoint - lastObjectTangentPosition;
                }
                if (i == BrushstrokeManager.brushstroke.Length - 1)
                {
                    var onLineSize = AxesUtils.GetAxisValue(size, settings.axisOrientedAlongTheLine)
                        + settings.gapSize;
                    var segmentSize = segmentDir.magnitude;
                    if (segmentSize > onLineSize) segmentDir = segmentDir.normalized * onLineSize;
                }


                var normal = -settings.projectionDirection;
                var otherAxes = AxesUtils.GetOtherAxes((AxesUtils.SignedAxis)(-settings.projectionDirection));
                var tangetAxis = otherAxes[settings.objectsOrientedAlongTheLine ? 0 : 1];
                Vector3 itemTangent = (AxesUtils.SignedAxis)(tangetAxis);
                var itemRotation = Quaternion.LookRotation(itemTangent, normal);
                var lookAt = Quaternion.LookRotation((Vector3)(AxesUtils.SignedAxis)
                    (settings.axisOrientedAlongTheLine), Vector3.up);
                if (segmentDir != Vector3.zero) itemRotation = Quaternion.LookRotation(segmentDir, normal) * lookAt;
                var pivotPosition = strokeItem.tangentPosition
                    - segmentDir.normalized * Mathf.Abs(AxesUtils.GetAxisValue(pivotToCenterLocal, tangetAxis));
                var itemPosition = pivotPosition + segmentDir / 2;

                var ray = new Ray(itemPosition + normal * height, -normal);
                if (settings.mode != LineSettings.PaintMode.ON_SHAPE)
                {
                    if (MouseRaycast(ray, out RaycastHit itemHit,
                        out GameObject collider, height * 2f, -1,
                        settings.paintOnPalettePrefabs, settings.paintOnMeshesWithoutCollider))
                    {
                        itemPosition = itemHit.point;
                        normal = itemHit.normal;
                    }
                    else if (settings.mode == LineSettings.PaintMode.ON_SURFACE) continue;
                }


                BrushSettings brushSettings = strokeItem.settings;
                if (settings.overwriteBrushProperties) brushSettings = settings.brushSettings;
                if (brushSettings.rotateToTheSurface && segmentDir != Vector3.zero)
                {
                    var tangent = segmentDir;
                    if (settings.parallelToTheSurface)
                    {
                        var plane = new Plane(normal, itemPosition);
                        tangent = plane.ClosestPointOnPlane(segmentDir + itemPosition) - itemPosition;
                    }
                    itemRotation = Quaternion.LookRotation(tangent, normal) * lookAt;
                }

                itemPosition += normal * brushSettings.surfaceDistance;
                itemRotation *= Quaternion.Euler(strokeItem.additionalAngle);
                if (brushSettings.embedInSurface && settings.mode != LineSettings.PaintMode.ON_SHAPE)
                {
                    var TRS = Matrix4x4.TRS(itemPosition, itemRotation, strokeItem.scaleMultiplier);
                    var bottomDistanceToSurfce = GetBottomDistanceToSurface(
                        strokeItem.settings.bottomVertices, TRS,
                        strokeItem.settings.height * strokeItem.scaleMultiplier.y,
                        settings.paintOnPalettePrefabs, settings.paintOnMeshesWithoutCollider);
                    if (!brushSettings.embedAtPivotHeight)
                        bottomDistanceToSurfce -= strokeItem.settings.bottomMagnitude;
                    itemPosition += itemRotation * new Vector3(0f, -bottomDistanceToSurfce, 0f);
                }
                itemPosition += itemRotation * brushSettings.localPositionOffset;
                var rootToWorld = Matrix4x4.TRS(itemPosition, itemRotation, strokeItem.scaleMultiplier)
                    * Matrix4x4.Translate(-prefab.transform.position);
                var itemScale = Vector3.Scale(prefab.transform.localScale, strokeItem.scaleMultiplier);
                var layer = settings.overwritePrefabLayer ? settings.layer : prefab.layer;
                Transform parentTransform = GetParent(settings, prefab.name, false, PersistentLineData.nextHexId);
                var paintItem = new PaintStrokeItem(prefab, itemPosition,
                    itemRotation * Quaternion.Euler(prefab.transform.eulerAngles), itemScale, layer, parentTransform);

                _paintStroke.Add(paintItem);
                PreviewBrushItem(prefab, rootToWorld, layer, camera);
            }
        }
    }
    #endregion
}
