using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum UIEvent
    {
        Click,
        Drag, //필요없나?
    }
    public enum MouseEvent
    {
        Press,
        Click,
        Drag,
    }
    public enum Scene
    {
        Unknown,
        Title,
        Main,
        Game,
    }

    public enum TitleSceneState
    {
        Title,
        //Login,
        //SignUp,
        //SignUpNickname,
        //SignUpNicknameCancle,
    }

    public enum Sound
    {
        Bgm,
        Effect,
        MaxCount,
    }

    public enum LoadingState
    {
        Loading,
        LoadingSuccess,
        LoadingFail,
    }
}
