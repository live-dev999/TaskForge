import { observer } from "mobx-react-lite";
import { Pagination as SemanticPagination } from "semantic-ui-react";
import { Pagination as PaginationModel } from "../models/pagination";

interface Props {
    pagination: PaginationModel | null;
    onPageChange: (pageNumber: number) => void;
    loading?: boolean;
}

export default observer(function Pagination({ pagination, onPageChange, loading = false }: Props) {
    if (!pagination || pagination.totalPages <= 1) {
        return null;
    }

    return (
        <SemanticPagination
            activePage={pagination.currentPage}
            totalPages={pagination.totalPages}
            onPageChange={(e, data) => {
                if (data.activePage && typeof data.activePage === 'number') {
                    onPageChange(data.activePage);
                }
            }}
            disabled={loading}
            firstItem={pagination.currentPage > 3 ? { content: '<<', icon: true } : null}
            lastItem={pagination.currentPage < pagination.totalPages - 2 ? { content: '>>', icon: true } : null}
            prevItem={pagination.currentPage > 1 ? { content: '<', icon: true } : null}
            nextItem={pagination.currentPage < pagination.totalPages ? { content: '>', icon: true } : null}
            pointing
            secondary
        />
    );
});

