using Photon.Pun;

public class TutorialGoodPerson : BaseBirdTarget
{
        public override void OnHit(float fac, PhotonMessageInfo info)
        {
                if (info.Sender.IsLocal)
                        Tutorial.instance.WarnOfPointLoss();
                base.OnHit(fac, info);
        }
}
