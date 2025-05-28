import { MarketValue } from "./market-value.model";

export interface KillsLine {
  opponent1KillsMain?: MarketValue;
  opponent2KillsMain?: MarketValue;
  totalKillsUnder?: MarketValue;
  totalKillsOver?: MarketValue;
  opponent1KillsHandicap?: MarketValue;
  opponent2KillsHandicap?: MarketValue;
}