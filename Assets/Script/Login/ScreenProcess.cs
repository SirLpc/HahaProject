using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using OneByOne;
using System.Runtime.InteropServices;
using System;

public class ScreenProcess : MonoBehaviour {

    public InputField acc;

    public InputField pwd;

    public RegisterPanel rp;

    IEnumerator Start() {
        //DisnableUtil.disnable(1024, 768);
        while (!NetWorkScript.Online)
        {
            yield return null;
        }

        yield return null;

        AutoLogin();
    }

    public void AutoLogin()
    {
        AccountDTO dto = new AccountDTO();
        dto.account = LocalPlayerPrefabs.GetId();
        dto.password = LocalPlayerPrefabs.GetPwd();
        NetWorkScript.Instance.write(Protocol.TYPE_LOGIN, -1, LoginProtocol.LOGIN_CREQ, dto);
    }






    public void loginClick() {
       if(acc.text==string.Empty)return;
       if(pwd.text==string.Empty)return;
       AccountDTO dto=new AccountDTO();
       dto.account=acc.text;
       dto.password=pwd.text;
       NetWorkScript.Instance.write(Protocol.TYPE_LOGIN,-1,LoginProtocol.LOGIN_CREQ,dto);
    }

   public void registerClick() {
       rp.open();
   }

   public void registerClose() {
       rp.closeClick();
   }

   public void homeClick() {
       
   }

   public void closeClick() {
       Application.Quit();
   }







}
