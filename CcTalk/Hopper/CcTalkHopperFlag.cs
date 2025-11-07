using System;

namespace CcTalk.Hopper;

[Flags]
public enum CcTalkHopperFlag : long
{
     /// <summary>Nothing to report.</summary>
    Nothing = 0,
    /// <summary>Absolute maximum current exceeded.</summary>
    CurrentExceeded = 1,
    /// <summary>Payout timeout  occured</summary>
    PayoutTimeout = 2,
    /// <summary>Motor reversed to clear a jam</summary>
    MotorReversed = 4,
    /// <summary>Opto fraud attempt, path blocked during idle.</summary>
    OptoIdleBlocked = 8,
    /// <summary>Opto fraud attempt, short-circuit during idle.</summary>
    OptoIdleShort = 16,
    /// <summary>Opto fraud attempt, blocked during payout.</summary>
    OptoPayoutBlocked = 32,
    /// <summary>Power-up detected.</summary>
    PowerUp = 64,
    /// <summary>Payout disabled.</summary>
    PayoutDisabled = 128,
    /// <summary>Opto fraud attempt, short-circuit during payout.</summary>
    OptoPayoutShort = 256,
    /// <summary>Single coin mode.</summary>
    SingleCoin = 512,
    /// <summary>Use other payout for remaining change.</summary>
    UseOtherPayout = 1024,
    /// <summary>Opto fraud attempt.</summary>
    OptFraud = 2048,
    /// <summary>Motor reverse limit reached.</summary>
    ReverseLimit = 4096,
    /// <summary>Inductive coil fault</summary>
    InductiveCoilFault = 8192,
    /// <summary>Power fail during non-volatile memory write.</summary>
    PowerFail = 16384,
    /// <summary>PIN number mechanism.</summary>
    PinNumber = 32768,
    /// <summary>Power down during payout</summary>
    PowerDownPayout = 65536,
    /// <summary>Unknown coin type paid out.</summary>
    UnknownCoin = 131072,
    /// <summary>PIN number incorrect.</summary>
    WrongPin = 262144,
    /// <summary>Cipher key incorrect</summary>
    WrongKey = 524288,
    /// <summary>Encryption enabled.</summary>
    Encryption = 1048576,
    /// <summary>Proprietary: card pending.</summary>
    CardPending = 8388608,
    /// <summary>X5: Hall sensor faulty.</summary>
    HallSensorError = 16777216,
    /// <summary>X5: Right pocket is blocked.</summary>
    RightPocketBlocked = 33554432,
    /// <summary>X5: Left pocket is blocked.</summary>
    LeftPocketBlocked = 67108864,
    /// <summary>X5: Coin didn't pass or stopped ont the recovery light barrier.</summary>
    CoinRecoveryError = 134217728,
    /// <summary>X5: Coin didn't pass or stopped ont the payment light barrier.</summary>
    CoinDeliveryError = 268435456,
    /// <summary>X5: Lack of polling during purge.</summary>
    PurgeTimeoutError = 536870912,
    /// <summary>X5: Sensor calibration is running.</summary>
    SensorCalibration = 1073741824,
    /// <summary>Payout was reset.</summary>
    Reset = 4294967296,
    /// <summary>Hopper is busy purging.</summary>
    Purging = 549755813888
}