#define PC // PC 플랫폼인지 확인하는 매크로
//#define Oculus

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ARAVRInput
{
    //public static Vector3 RHandPosition;
    //public static Vector3 RHandDirection;
    //public static Vector3 LHandPosition;
    //public static Vector3 LHandDirection;
    // public static Vector3 LHand;
    // public static Vector3 RHand;

    // 왼쪽 컨트롤러
    static Transform lHand;

    public static Transform LHand
    {
        get
        {
            if (lHand == null)
            {
#if PC // PC 플랫폼일 경우 실행
                // LHand 라는 게임오브젝트를 만든다
                GameObject handObj = new GameObject("LHand");
                // 만들어진 객체의 트랜스폼을 lHand에 할당
                lHand = handObj.transform;
                // 컨트롤러를 카메라의 자식 객체로 등록
                lHand.parent = Camera.main.transform;
#elif Oculus
                lHand = GameObject.Find("LeftControllerAnchor").transform;
#endif // 이 사이에 있는거만
            }
            return lHand;
        }
    }

    public static Vector3 LHandPosition
    {
        get
        {
#if PC 
            // 마우스로 스크린 좌표 얻어오기
            Vector3 pos = Input.mousePosition;
            // z 값은 0.7m 로 설정
            pos.z = 0.7f;
            // 스크린좌료를 월드좌표로 반환
            pos = Camera.main.ScreenToWorldPoint(pos);
            LHand.position = pos;
            return pos;
#elif Oculus
            Vector3 pos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
            pos = GetTransform().TransformPoint(pos);
            return pos;
#endif
        }
    }

    public static Vector3 LHandDirection
    {
        get
        {
#if PC // PC 플랫폼일 경우 실행
            Vector3 dir = LHandPosition - Camera.main.transform.position;
            LHand.forward = dir;
            return dir;
#elif Oculus
            Vector3 dir = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch) * Vector3.forward;
            dir = GetTransform().TransformDirection(dir);
            return dir;
#endif // 이 사이에 있는거만
        }
    }

    // 오른쪽 컨트롤러
    static Transform rHand;

    public static Transform RHand
    {
        get
        {
            if (rHand == null)
            {
#if PC
                GameObject handObj = new GameObject("RHand");
                rHand = handObj.transform;
                rHand.parent = Camera.main.transform;
#elif Oculus
                lHand = GameObject.Find("RightControllerAnchor").transform;
#endif
            }
            return rHand;
        }
    }

    public static Vector3 RHandPosition
    {
        get
        {
#if PC 
            // 마우스로 스크린 좌표 얻어오기
            Vector3 pos = Input.mousePosition;
            // z 값은 0.7m 로 설정
            pos.z = 0.7f;
            // 스크린좌료를 월드좌표로 반환
            pos = Camera.main.ScreenToWorldPoint(pos);
            RHand.position = pos;
            return pos;
#elif Oculus
            Vector3 pos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
            pos = GetTransform().TransformPoint(pos);
            return pos;
#endif
        }
    }

    public static Vector3 RHandDirection
    {
        get
        {
#if PC // PC 플랫폼일 경우 실행
            Vector3 dir = RHandPosition - Camera.main.transform.position;
            LHand.forward = dir;
            return dir;
#elif Oculus
            Vector3 dir = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch) * Vector3.forward;
            dir = GetTransform().TransformDirection(dir);
            return dir;
#endif // 이 사이에 있는거만
        }
    }

#if Oculus
    static Transform rootTransForm;
#endif

#if Oculus
    static Transform GetTransform()
    {
        if (rootTransForm == null)
        {
            rootTransForm = GameObject.Find("TrackingSpace").transform;
        }
        return rootTransForm;
    }
#endif

#if PC
    public enum ButtonTarget
    {
        Fire1,
        Fire2,
        Fire3,
        Jump
    }
#endif
    public enum Button
    {
#if PC
        One = ButtonTarget.Fire1,
        Two = ButtonTarget.Jump,
        Thumbstick = ButtonTarget.Fire1,
        IndexTrigger = ButtonTarget.Fire3,
        HandTrigger = ButtonTarget.Fire2
#elif Oculus
        One = OVRInput.Button.One,
        Two = OVRInput.Button.Two,
        Thumbstick = OVRInput.Button.PrimaryThumbstick,
        IndexTrigger = OVRInput.Button.PrimaryIndexTrigger,
        HandTrigger = OVRInput.Button.PrimaryHandTrigger
#endif
    }

    public enum Controller
    {
#if PC
        LTouch,
        RTouch
#elif Oculus
        LTouch = OVRInput.Controller.LTouch,
        RTouch = OVRInput.Controller.RTouch
#endif
    }

    // 컨트롤러의 특정 버튼을 누르고 있는동안 true를 반환
    public static bool Get(Button virtualMask, Controller hand = Controller.RTouch)
    {
#if PC
        // virtualMask 에 들어온 값을 ButtonTarget 타입으로 변환해 전달한다.
        return Input.GetButton(((ButtonTarget)virtualMask).ToString());
#elif Oculus
        return OVRInput.Get((OVRInput.Button)virtualMask, (OVRInput.Controller)hand);
#endif
    }

    // 컨트롤러의 특정 버튼을 눌렀을 때 true를 반환
    public static bool GetDown(Button virtualMask, Controller hand = Controller.RTouch)
    {
#if PC
        // virtualMask 에 들어온 값을 ButtonTarget 타입으로 변환해 전달한다.
        return Input.GetButtonDown(((ButtonTarget)virtualMask).ToString());
#elif Oculus
        return OVRInput.GetDown((OVRInput.Button)virtualMask, (OVRInput.Controller)hand);
#endif
    }

    // 컨트롤러의 특정 버튼을 눌렀다 땠을 때 true를 반환
    public static bool GetUp(Button virtualMask, Controller hand = Controller.RTouch)
    {
#if PC
        // virtualMask 에 들어온 값을 ButtonTarget 타입으로 변환해 전달한다.
        return Input.GetButtonUp(((ButtonTarget)virtualMask).ToString());
#elif Oculus
        return OVRInput.GetUp((OVRInput.Button)virtualMask, (OVRInput.Controller)hand);
#endif
    }

    // 컨트롤러의 Axis 입력을 반환
    // axis : Horizontal, Vertical 값을 갖는다.
    public static float GetAxis(string axis, Controller hand = Controller.LTouch)
    {
#if PC
        return Input.GetAxis(axis);
#elif Oculus
        if (axis == "Horizontal")
        {
            return OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, (OVRInput.Controller)hand).x;
        }
        else
        {
            return OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, (OVRInput.Controller)hand).y;
        }
#endif
    }

    // 카메라가 바라보는 방향을 기준으로 센터를 잡는다.
    public static void Recenter(Transform target, Vector3 direction)
    {
        target.forward = target.rotation * direction;
#if Oculus
        OVRManager.display.RecenterPose();
#endif
    }

#if PC
    static Vector3 originScale = Vector3.one * 0.02f;
#else
    static Vector3 originScale = Vector3.one * 0.005f;
#endif

    // 광선레이가 닿는곳에 크로스헤어를 위치시키고 싶다
    public static void DrawCrosshair(Transform crosshair, bool isHand = true, Controller hand = Controller.RTouch)
    {
        Ray ray;
        // 컨트롤러의 위치와 방향을 이용해 레이 제작
        if (isHand)
        {
#if PC
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
#else
            if(hand == Controller.RTouch)
            {
                ray = new Ray(RHandPosition, RHandDirection);
            }
            else
            {
                ray = new Ray(LHandPosition, LHandDirection);
            }
#endif
        }
        else
        {
            // 카메라를 기준으로 화면의 정중앙으로 레이를 제작
            ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        }
        // 크로스헤어 그리기
        // 눈에 안보이는 Plane을 만든다
        Plane plane = new Plane(Vector3.up, 0);
        float distance = 0;
        // plane을 이용해 ray를 쓴다.
        if(plane.Raycast(ray, out distance))
        {
            // 레이의 GetPoint 함수를 이용해 충돌 지점의 위치를 가져온다.
            crosshair.position = ray.GetPoint(distance);
            crosshair.forward = -Camera.main.transform.forward;

            // 크롯헤어의 크기를 최소 기본 크기에서 거리에 따라 더 커지도록 한다.
            crosshair.localScale = originScale * Mathf.Max(1, distance);
        }
        else
        {
            crosshair.position = ray.origin + (ray.direction * 100);
            crosshair.forward = -Camera.main.transform.forward;
            distance = (crosshair.position - ray.origin).magnitude;
            crosshair.localScale = originScale * Mathf.Max(1, distance);
        }
    }

}
