import { MetaData } from "../../../shared/interfaces/meta-data";
import { PagedList } from "../../../shared/interfaces/paged-list";
import { UserGet } from "../../entities/user-get.model";

export interface PagedUserResponse extends PagedList<UserGet> {}