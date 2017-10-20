using Commom.Dto;
using Common.Code;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Logic
{
    public class AccountReceive : IReceiveHandler
    {
        public void OnDoResponse(OperationResponse response)
        {
            AccountCode subCode = (AccountCode)response.Parameters[50];
            switch (subCode)
            {
                case AccountCode.Login:
                    login(response);
                    break;
                case AccountCode.Register:
                    register(response);
                    break;
                case AccountCode.Logout:
                    logout(response);
                    break;
            }
        }

        void login(OperationResponse response)
        {
            Scene01Manager.Ins.Excute4Login(response);
        }

        void register(OperationResponse response)
        {
            Scene01Manager.Ins.ShowRegisterInfo(response.DebugMessage);
        }

        void logout(OperationResponse response)
        {
            Scene01Manager.Ins.Excute4BackToLoginPanel();
        }
    }
}
