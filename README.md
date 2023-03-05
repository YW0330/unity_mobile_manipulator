# Unity with Mobile Manipulator in Oculus Quest 2
ROS Noetic 環境下，使 Unity 與移動機械手臂同步操作，其中移動機械手臂由 Kinva Gen3 和東元移動平台組成，最後呈現在 Oculus Quest 2 之中。

- [開發文件](https://hackmd.io/kZdWdeeBRZKJ2jkXbACzWw)
- 操作方法
    1. git 複製一份專案到本地端
        ```shell
        $ git clone https://github.com/YW0330/unity_mobile_manipulator.git
        ```
    2. 使用 Unity Hub 開啟 `ros_oculus` 資料夾，即 Unity 專案

:warning: `URDF Importer` 需要進行移除，再燒錄至 Oculus Quest 2。