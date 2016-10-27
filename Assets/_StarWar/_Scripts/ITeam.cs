using System;
using System.Collections.Generic;

interface ITeam
{
    bool IsEnemy { get; set; }
    void InitEnemy();
    void InitFriend();
}
