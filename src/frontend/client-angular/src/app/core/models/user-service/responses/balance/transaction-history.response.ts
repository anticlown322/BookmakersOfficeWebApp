import { PagedList } from '../../../shared/interfaces/paged-list';
import { Transaction } from '../../entities/transaction.model';

export type TransactionHistoryResponse = PagedList<Transaction>;