import { MetaData } from '../../../shared/interfaces/meta-data';
import { Tournament } from './tournament.model';

export interface PrematchUpdate {
    tournaments: Tournament[];
    metaData: MetaData;
}
