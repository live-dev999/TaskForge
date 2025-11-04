import { observer } from "mobx-react-lite";
import { Pagination as SemanticPagination } from "semantic-ui-react";
import { Pagination as PaginationModel } from "../models/pagination";

interface Props {
    pagination: PaginationModel | null;
    onPageChange: (pageNumber: number) => void;
    loading?: boolean;
}

export default observer(function Pagination({ pagination, onPageChange, loading = false }: Props) {
    // Debug logging
    console.log('Pagination component render:', {
        hasPagination: !!pagination,
        currentPage: pagination?.currentPage,
        totalPages: pagination?.totalPages,
        totalItems: pagination?.totalItems,
        itemsPerPage: pagination?.itemsPerPage
    });

    if (!pagination) {
        console.warn('Pagination component: pagination is null');
        return null;
    }

    // Show pagination even for 1 page to display information
    // But warn if there's only one page
    if (pagination.totalPages <= 1) {
        console.log('Pagination component: totalPages <= 1, but showing anyway for info');
        // Still show pagination to display page info, but it won't be clickable
    }

    // For debugging: show pagination info
    console.log('Rendering SemanticPagination with:', {
        activePage: pagination.currentPage,
        totalPages: pagination.totalPages
    });

    return (
        <div>
            {/* Debug info - remove later */}
            <div style={{ marginBottom: '10px', fontSize: '12px', color: '#666' }}>
                Страница {pagination.currentPage} из {pagination.totalPages} (всего: {pagination.totalItems} элементов)
            </div>
            <SemanticPagination
                activePage={pagination.currentPage}
                totalPages={pagination.totalPages}
                onPageChange={(e, data) => {
                    console.log('Pagination clicked:', data);
                    if (data.activePage && typeof data.activePage === 'number') {
                        onPageChange(data.activePage);
                    }
                }}
                disabled={loading}
                // Display settings - show page numbers
                // boundaryRange: pages to show at start/end (1 = show first and last)
                // siblingRange: pages to show around current (1 = show 1 page on each side)
                boundaryRange={1}
                siblingRange={1}
                // Show navigation arrows (default icons)
                prevItem={pagination.currentPage > 1}
                nextItem={pagination.currentPage < pagination.totalPages}
                // Style
                pointing
                secondary
            />
        </div>
    );
});

