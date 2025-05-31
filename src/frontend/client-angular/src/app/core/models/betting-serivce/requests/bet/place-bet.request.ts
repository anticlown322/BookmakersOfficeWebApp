import { MarketValue } from "../../../sport-data-service/entities/match/market-value.model";

export interface PlaceBetRequest {
  matchId: string;
  amount: number;
  lineType: string;
  marketSelection: string;
  odds: string;
}