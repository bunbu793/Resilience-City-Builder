using UnityEngine;

public class CameraController : MonoBehaviour
{
    // =========================
    // 移動設定
    // =========================

    [Header("移動")]
    public float moveSpeed = 10f;


    // =========================
    // ズーム設定
    // =========================

    [Header("ズーム")]
    public float zoomSpeed = 30f;

    // FOVの最小値
    // 小さいほどズームイン
    public float minFOV = 20f;

    // FOVの最大値
    // 大きいほどズームアウト
    public float maxFOV = 80f;


    // =========================
    // 回転設定
    // =========================

    [Header("回転")]
    public float rotateSpeed = 3f;

    // R / Lキーによる回転速度
    public float keyRotateSpeed = 90f;

    // 上下回転の範囲
    public float minPitch = -80f;
    public float maxPitch = 80f;


    // =========================
    // 回転角度
    // =========================

    // 水平方向
    private float yaw;

    // 上下方向
    private float pitch;


    // =========================
    // 初期状態
    // =========================

    private Vector3 startPosition;
    private float startYaw;
    private float startPitch;
    private float startFOV;


    // =========================
    // 初期化
    // =========================

    void Start()
    {
        // 初期位置
        startPosition = transform.position;

        // 現在の回転角度を取得
        Vector3 angles = transform.eulerAngles;

        yaw = angles.y;

        // Unityの0～360°を-180～180°に変換
        pitch = angles.x;

        if (pitch > 180f)
        {
            pitch -= 360f;
        }

        // 初期角度を保存
        startYaw = yaw;
        startPitch = pitch;

        // Cameraを取得
        Camera cam = GetComponent<Camera>();

        // 初期FOV
        startFOV = cam.fieldOfView;
    }


    // =========================
    // 毎フレーム実行
    // =========================

    void Update()
    {
        MoveCamera();

        ZoomCamera();

        RotateCamera();

        KeyboardRotate();

        ResetCamera();
    }


    // =========================
    // カメラ移動
    // =========================

    void MoveCamera()
    {
        // WASD・矢印キー
        float horizontal =
            Input.GetAxisRaw("Horizontal");

        float vertical =
            Input.GetAxisRaw("Vertical");


        // カメラの前方向
        Vector3 forward = transform.forward;

        // カメラの右方向
        Vector3 right = transform.right;


        // 上下方向の移動を無効化
        forward.y = 0f;
        right.y = 0f;


        // 正規化
        forward.Normalize();
        right.Normalize();


        // 移動方向
        Vector3 direction =
            forward * vertical +
            right * horizontal;


        // 斜め移動が速くなりすぎないようにする
        if (direction.magnitude > 1f)
        {
            direction.Normalize();
        }


        // カメラを移動
        transform.position +=
            direction *
            moveSpeed *
            Time.deltaTime;
    }


    // =========================
    // ズーム
    // =========================

    void ZoomCamera()
    {
        float zoomInput = 0f;


        // -------------------------
        // マウスホイール
        // -------------------------

        zoomInput +=
            Input.GetAxis("Mouse ScrollWheel") * 10f;


        // -------------------------
        // Z = ズームイン
        // -------------------------

        if (Input.GetKey(KeyCode.Z))
        {
            zoomInput += 1f;
        }


        // -------------------------
        // Q = ズームアウト
        // -------------------------

        if (Input.GetKey(KeyCode.Q))
        {
            zoomInput -= 1f;
        }


        // 入力がなければ終了
        if (Mathf.Approximately(zoomInput, 0f))
        {
            return;
        }


        // Camera取得
        Camera cam = GetComponent<Camera>();


        // FOV変更
        cam.fieldOfView -=
            zoomInput *
            zoomSpeed *
            Time.deltaTime;


        // FOV制限
        cam.fieldOfView =
            Mathf.Clamp(
                cam.fieldOfView,
                minFOV,
                maxFOV
            );
    }


    // =========================
    // マウスによる回転
    // =========================

    void RotateCamera()
    {
        // 右クリック中だけ回転
        if (!Input.GetMouseButton(1))
        {
            return;
        }


        // -------------------------
        // マウス入力
        // -------------------------

        float mouseX =
            Input.GetAxis("Mouse X") *
            rotateSpeed;

        float mouseY =
            Input.GetAxis("Mouse Y") *
            rotateSpeed;


        // -------------------------
        // 水平方向
        // -------------------------

        yaw += mouseX;


        // -------------------------
        // 上下方向
        // -------------------------

        pitch -= mouseY;


        // 上下回転を制限
        pitch =
            Mathf.Clamp(
                pitch,
                minPitch,
                maxPitch
            );


        ApplyRotation();
    }


    // =========================
    // R / Lキーによる回転
    // =========================

    void KeyboardRotate()
    {
        float rotation = 0f;


        // R = 右回転
        if (Input.GetKey(KeyCode.R))
        {
            rotation += keyRotateSpeed;
        }


        // L = 左回転
        if (Input.GetKey(KeyCode.L))
        {
            rotation -= keyRotateSpeed;
        }


        // 回転
        if (!Mathf.Approximately(rotation, 0f))
        {
            yaw +=
                rotation *
                Time.deltaTime;

            ApplyRotation();
        }
    }


    // =========================
    // 回転を適用
    // =========================

    void ApplyRotation()
    {
        // 水平方向は0～360°に整理
        yaw = Mathf.Repeat(yaw, 360f);


        // 上下方向を制限
        pitch =
            Mathf.Clamp(
                pitch,
                minPitch,
                maxPitch
            );

        // カメラに回転を適用
        transform.rotation =
            Quaternion.Euler(
                pitch,
                yaw,
                0f
            );
    }

    public void SetYaw(float targetYaw)
    {
        yaw = Mathf.Repeat(targetYaw, 360f);
        ApplyRotation();
    }

    // =========================
    // Tキーで初期状態に戻す
    // =========================

    void ResetCamera()
    {
        if (!Input.GetKeyDown(KeyCode.T))
        {
            return;
        }


        // 位置を戻す
        transform.position =
            startPosition;


        // 回転を戻す
        yaw =
            startYaw;

        pitch =
            startPitch;

        ApplyRotation();


        // FOVを戻す
        Camera cam =
            GetComponent<Camera>();

        cam.fieldOfView =
            startFOV;
    }
}