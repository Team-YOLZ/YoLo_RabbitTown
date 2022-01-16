using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum MeadowUnit //초원유닛 
    {
        Null,
        Chicken1, Cow1, Duck1, Horse1, Sheep1, Goat1, //1~6
        Chicken2, Cow2, Duck2, Horse2, Sheep2, Goat2, //7~12
        Chicken3, Cow3, Duck3, Horse3, //13~16
        Boss1,
    }
    public enum CreatureState
    {
        Idle,
        Moving,
        Skill,
        Dead,
    }
    public enum WorldObject
    {
        Unknown,
        Tree,
        Grass,
        Flower,
        Rock,
        Well,
        Cloud,
        Bush,
    }
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
