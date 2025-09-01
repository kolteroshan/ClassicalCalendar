namespace Records;

public record MarginRecord(
    decimal TotalUsedMoney,
    decimal UsedMoneyForSell,
    decimal UsedMoneyForBuy,
    decimal HedgeMoney);