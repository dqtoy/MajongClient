using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commom.Dto;
using Common.Code;
using ExitGames.Client.Photon;

namespace Assets.Scripts.Logic
{
    public class DetailReceive : IReceiveHandler
    {
        public void OnDoResponse(OperationResponse response)
        {
            DetailCode subCode = (DetailCode) response.Parameters[50];
            Scene01Manager.Ins.ShowLog(subCode);
            switch (subCode)
            {
                case DetailCode.UpdateInfo:
                    updateInfo(response);
                    break;
                case DetailCode.LoadImage:
                    break;
            }
        }

        void updateInfo(OperationResponse response)
        {
            if (response.ReturnCode == 0)
            {
                DetailDto dto = LitJson.JsonMapper.ToObject<DetailDto>(response.Parameters[0].ToString());
                Scene01Manager.Ins.UpdateHeadAndNickName(dto);
            }
        }
    }
}
