import { PagedList } from '../../../shared/interfaces/paged-list';
import { TransactionDto } from '../../entities/transaction.dto';

export type TransactionHistoryResponse = PagedList<TransactionDto>;