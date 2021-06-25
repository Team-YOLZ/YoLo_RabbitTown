using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum MeadowUnit //초원유닛 
    {
        Null, 
        Chicken1, Chicken2, Chicken3,
        Cow1, Cow2, Cow3,
        Duck1, Duck2, Duck3,
        Horse1, Horse2, Horse3,
        Sheep1, Sheep2,
        Goat1, Goat2,
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
