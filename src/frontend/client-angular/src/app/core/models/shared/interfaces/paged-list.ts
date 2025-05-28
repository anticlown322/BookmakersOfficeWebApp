import { MetaData } from "./meta-data";

export interface PagedList<T> {
    items: T[];
    metaData: MetaData;
}