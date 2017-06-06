
public class SpaceNotifications
{

    public const string ShipCreated = "ship.created"; //NetSpaceShip thisShip, bool isMyShip
    public const string ShipRespawned = "ship.respawned"; //NetSpaceShip thisShip, bool isMyShip

    public const string SpeedUp = "myship.speedup"; //bool enalbeSpeedUp

    public const string CreatBullet = "bullet.creat";

    /// <summary>
    /// NetSpaceShip damagedShip,  int damage
    /// </summary>
    public const string BulletAttakOn = "bullet.attackon";

    /// <summary>
    /// int damage,  int attackerShipId
    /// </summary>
    public const string ShipTakeDamage = "ship.takedamage";


}
