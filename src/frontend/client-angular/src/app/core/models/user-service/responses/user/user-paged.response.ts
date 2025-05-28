import { MetaData } from "../../../shared/interfaces/meta-data";
import { UserGetDto } from "../../entities/user-get.dto";

export interface UserPagedResponse {
    items: UserGetDto[];
    metaData: MetaData;
}