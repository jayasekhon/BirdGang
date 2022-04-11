using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class TutorialGoodPerson : BaseBirdTarget
{
        public override void OnHit(PhotonMessageInfo info)
        {
                if (info.Sender.IsLocal)
                        Tutorial.instance.WarnOfPointLoss();
                base.OnHit(info);
        }
}
