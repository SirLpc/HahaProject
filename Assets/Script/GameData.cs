using OneByOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


   public class GameData
   {
       public static List<ErrorModel> errors = new List<ErrorModel>();
       public static UserDTO user = null;

       public static Vector3 teamOneStart = new Vector3(143.6f, 7.3f, 16.4f);

       public static Vector3 teamTwoStart = new Vector3(17.3f, 7.3f, 143.27f);

       public static RectTransform Par;

       static float baseOffset = 5.2f;
       static float baseWH = 1024f / 768;

       public static Vector3 worldToTarget(Vector3 world) {
           return Camera.main.WorldToScreenPoint(world + getOffset());
           
       }

       public static Vector3 getOffset()
       {
           return new Vector3(0,-3, Par.rect.width / Par.rect.height / baseWH * baseOffset);
       }
    }

