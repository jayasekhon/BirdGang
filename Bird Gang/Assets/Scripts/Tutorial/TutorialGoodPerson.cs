using System;
using System.Collections;
using System.Collections.Generic;
using Codice.Client.Commands.TransformerRule;
using Photon.Pun;
using UnityEngine;

public class TutorialGoodPerson : BaseBirdTarget
{
        [PunRPC]
        public new void OnHit(PhotonMessageInfo info)
        {
                if (info.Sender.IsLocal)
                        Tutorial.instance.WarnOfPointLoss();
                base.OnHit(info);
        }
}
