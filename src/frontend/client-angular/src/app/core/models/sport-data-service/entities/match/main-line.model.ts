import { MarketValue } from "./market-value.model";

export interface MainLine {
  opponent1Win?: MarketValue;
  opponent2Win?: MarketValue;
  draw?: MarketValue;
  opponent1WinOrDraw?: MarketValue;
  opponent2WinOrDraw?: MarketValue;
}