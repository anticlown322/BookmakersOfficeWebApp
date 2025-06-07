import { MetaData } from '../../../shared/interfaces/meta-data';
import { TournamentResult } from './tournament-result.model';

export interface ResultsUpdate {
    tournaments: TournamentResult[];
    metaData: MetaData;
}
