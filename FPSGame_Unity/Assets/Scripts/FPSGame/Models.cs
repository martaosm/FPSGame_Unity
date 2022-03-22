using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Models
{

    #region - Player -

    public enum PlayerStance
    {
        Stand,
        Crouch,
        Prone
    }
    [Serializable]
    public class PlayerSettingsModel
    {
        [Header("View Settings")]
        public float ViewXSensitivity;
        public float ViewYSensitivity;
        public bool ViewXInverted;
        public bool ViewYInverted;

        [Header("Movement")] 
        public float WalkingForwardSpeed;
        public float WalkingStrafeSpeed;
        public float WalkingBackwardSpeed;

        [Header("Jumping")] public float JumpingHeight;
        public float jumpingFalloff;


    }

    #endregion
}
