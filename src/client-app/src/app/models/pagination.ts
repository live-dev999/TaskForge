export interface Pagination {
    currentPage: number;
    totalPages: number;
    pageSize: number;
    totalCount: number;
}

export interface PagedResult<T> {
    items: T[];
    pagination: Pagination;
}

