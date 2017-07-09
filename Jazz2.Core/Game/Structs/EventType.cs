﻿namespace Jazz2.Game.Structs
{
    // ToDo: Reassign numbers to events, it's a bit random now...

    public enum EventType : ushort
    {
        Empty = 0x0000,

        Generator = 0x1000,

        // Basic
        LevelStart = 0x0100,
        LevelStartMP = 0x0101,
        SavePoint = 0x005A,

        // Scenery
        SceneryDestruct = 0x0116,
        SceneryDestructButtstomp = 0x0117,
        SceneryDestructSpeed = 0x0118,
        SceneryCollapse = 0x0119,

        // Modifiers
        ModifierVine = 0x0110,
        ModifierOneWay = 0x0111,
        ModifierHook = 0x0112,
        ModifierHPole = 0x0113,
        ModifierVPole = 0x0114,
        ModifierHurt = 0x0115,
        ModifierTube = 0x0144,
        ModifierRicochet = 0x0145,
        ModifierSlide = 0x0147,
        ModifierDeath = 0x0148,
        ModifierSetWater = 0x0149,

        // Area
        AreaStopEnemy = 0x0143,
        AreaFloatUp = 0x0146,
        AreaHForce = 0x0401,
        AreaText = 0x014A,
        AreaEOL = 0x0108,
        AreaCallback = 0x0109,
        AreaActivateBoss = 0x010A,
        AreaFlyOff = 0x010B,
        AreaRevertMorph = 0x010C,

        // Triggers
        TriggerCrate = 0x0060,
        TriggerArea = 0x011A,
        TriggerZone = 0x011B,

        // Warp
        WarpCoinBonus = 0x010D,
        WarpOrigin = 0x010E,
        WarpTarget = 0x010F,

        // Lights
        LightSet = 0x0120,
        LightReset = 0x0124,
        LightSteady = 0x0121,
        LightPulse = 0x0122,
        LightFlicker = 0x0123,

        // Environment
        Spring = 0x00C0,
        Bridge = 0x00C3,
        MovingPlatform = 0x00C4,
        PushableBox = 0x00C5,
        Eva = 0x0500,
        Pole = 0x0501,
        SignEOL = 0x0502,
        Moth = 0x0503,
        SteamNote = 0x0504,
        Bomb = 0x0505,
        PinballBumper = 0x0506,
        PinballPaddle = 0x0507,

        Weather = 0x0510,
        AmbientSound = 0x0511,
        AmbientBubbles = 0x0512,

        // Enemies
        EnemyTurtle = 0x0180,
        EnemyLizard = 0x0185,
        EnemySucker = 0x018C,
        EnemySuckerFloat = 0x018D,
        EnemyLabRat = 0x018B,
        EnemyHelmut = 0x018E,
        EnemyDragon = 0x018F,
        EnemyBat = 0x0190,
        EnemyFatChick = 0x0191,
        EnemyFencer = 0x0192,
        EnemyRapier = 0x0193,
        EnemySparks = 0x0194,
        EnemyMonkey = 0x0195,
        EnemyDemon = 0x0196,
        EnemyBee = 0x0197,
        EnemyBeeSwarm = 0x0198,
        EnemyCaterpillar = 0x0199,
        EnemyCrab = 0x019A,
        EnemyDoggy = 0x019B,
        EnemyDragonfly = 0x019C,
        EnemyFish = 0x019D,
        EnemyMadderHatter = 0x019E,
        EnemyRaven = 0x019F,
        EnemySkeleton = 0x0200,
        EnemyTurtleTough = 0x0201,
        EnemyTurtleTube = 0x0202,
        EnemyWitch = 0x0204,
        EnemyLizardFloat = 0x0205,

        BossTweedle = 0x0203,
        BossBilsy = 0x0210,
        BossDevan = 0x0211,
        BossQueen = 0x0212,
        BossRobot = 0x0213,
        BossUterus = 0x0214,
        BossTurtleTough = 0x0215,
        BossBubba = 0x0216,
        BossDevanRemote = 0x0217,
        BossBolly = 0x0218,

        TurtleShell = 0x0182,

        // Collectibles
        Coin = 0x0048,
        Gem = 0x0040,
        GemGiant = 0x0041,
        Carrot = 0x0050,
        CarrotFly = 0x0051,
        CarrotInvincible = 0x0052,
        OneUp = 0x0055,
        FastFire = 0x0001,

        // Weapons
        AmmoBouncer = 0x0002,
        AmmoFreezer = 0x0003,
        AmmoSeeker = 0x0004,
        AmmoRF = 0x0005,
        AmmoToaster = 0x0006,
        AmmoTNT = 0x0007,
        AmmoPepper = 0x0008,
        AmmoElectro = 0x0009,
        PowerUpBlaster = 0x0011,
        PowerUpBouncer = 0x0012,
        PowerUpFreezer = 0x0013,
        PowerUpSeeker = 0x0014,
        PowerUpRF = 0x0015,
        PowerUpToaster = 0x0016,
        PowerUpTNT = 0x0017,
        PowerUpPepper = 0x0018,
        PowerUpElectro = 0x0019,

        // Food
        FoodApple = 0x008D,
        FoodBanana = 0x008E,
        FoodCherry = 0x008F,
        FoodOrange = 0x0090,
        FoodPear = 0x0091,
        FoodPretzel = 0x0092,
        FoodStrawberry = 0x0093,
        FoodLemon = 0x009A,
        FoodLime = 0x009B,
        FoodThing = 0x009C,
        FoodWaterMelon = 0x009D,
        FoodPeach = 0x009E,
        FoodGrapes = 0x009F,
        FoodLettuce = 0x00A0,
        FoodEggplant = 0x00A1,
        FoodCucumber = 0x00A2,
        FoodPepsi = 0x00A3,
        FoodCoke = 0x00A4,
        FoodMilk = 0x00A5,
        FoodPie = 0x00A6,
        FoodCake = 0x00A7,
        FoodDonut = 0x00A8,
        FoodCupcake = 0x00A9,
        FoodChips = 0x00AA,
        FoodCandy = 0x00AB,
        FoodChocolate = 0x00AC,
        FoodIceCream = 0x00AD,
        FoodBurger = 0x00AE,
        FoodPizza = 0x00AF,
        FoodFries = 0x00B0,
        FoodChickenLeg = 0x00B1,
        FoodSandwich = 0x00B2,
        FoodTaco = 0x00B3,
        FoodHotDog = 0x00B4,
        FoodHam = 0x00B5,
        FoodCheese = 0x00B6,

        // Containers
        Crate = 0x0061,
        Barrel = 0x0062,
        CrateAmmo = 0x0063,
        BarrelAmmo = 0x0064,
        CrateGem = 0x0065,
        BarrelGem = 0x0066,

        PowerUpMorph = 0x0067,

        AirboardGenerator = 0x0068
    }
}