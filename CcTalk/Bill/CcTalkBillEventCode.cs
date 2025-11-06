namespace CcTalk.Bill;

public enum CcTalkBillEventCode
{
    MasterInhibitActive = 0,
	BillReturnedFromEscrow = 1,
	InvalidBillValidationFail = 2,
	InvalidBillTransportProblem = 3,
	InhibitedBillOnSerial = 4,
	InhibitedBillOnDipSwitches = 5,
	BillJammedInTransportUnsafeMode = 6,
	BillJammedInStacker = 7,
	BillPulledBackwards = 8,
	BillTamper = 9,
	StackerOk = 10,
	StackerRemoved = 11,
	StackerInserted = 12,
	StackerFaulty = 13,
	StackerFull = 14,
	StackerJammed = 15,
	BillJammedInTransportSafeMode = 16,
	OptoFraudDetected = 17,
	StringFraudDetected = 18,
	AntiStringMechanismFaulty = 19,
	BarcodeDetected = 20,
	UnknownBillTypeStacked = 21
}