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


    // =========================
    // 初期状態
    // =========================

    private Vector3 startPosition;
    private Quaternion startRotation;
    private float startFOV;


    // =========================
    // 初期化
    // =========================

    void Start()
    {
        // ゲーム開始時のカメラ位置を保存
        startPosition = transform.position;

        // ゲーム開始時のカメラ角度を保存
        startRotation = transform.rotation;

        // Cameraコンポーネントを取得
        Camera cam = GetComponent<Camera>();

        // ゲーム開始時のFOVを保存
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


        // 入力がなければ何もしない
        if (Mathf.Approximately(zoomInput, 0f))
        {
            return;
        }


        // Cameraコンポーネント取得
        Camera cam = GetComponent<Camera>();


        // -------------------------
        // FOVを変更
        // -------------------------

        cam.fieldOfView -=
            zoomInput *
            zoomSpeed *
            Time.deltaTime;


        // -------------------------
        // ズーム範囲制限
        // -------------------------

        cam.fieldOfView =
            Mathf.Clamp(
                cam.fieldOfView,
                minFOV,
                maxFOV
            );
    }


    // =========================
    // マウスでカメラ回転
    // =========================

    void RotateCamera()
    {
        // 右クリック中だけ回転
        if (Input.GetMouseButton(1))
        {
            // マウス移動量
            float mouseX =
                Input.GetAxis("Mouse X") *
                rotateSpeed;

            float mouseY =
                Input.GetAxis("Mouse Y") *
                rotateSpeed;


            // 上下回転
            transform.Rotate(
                -mouseY,
                0f,
                0f,
                Space.Self
            );


            // 左右回転
            transform.Rotate(
                0f,
                mouseX,
                0f,
                Space.World
            );
        }
    }


    // =========================
    // Rキーで初期状態に戻す
    // =========================

    void ResetCamera()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            // 位置を戻す
            transform.position =
                startPosition;

            // 回転を戻す
            transform.rotation =
                startRotation;


            // FOVを戻す
            Camera cam =
                GetComponent<Camera>();

            cam.fieldOfView =
                startFOV;
        }
    }
}