import { observer } from "mobx-react-lite";
import { Grid, Segment } from "semantic-ui-react";
import { useStore } from "../../../app/stores/store";
import TaskItemList from "./TaskItemList";
import { useEffect, useState } from "react";
import LoadingComponent from "../../../app/layout/LoadingComponent";
import Pagination from "../../../app/components/Pagination";

export default observer(function TaskItemDashboard() {
    const { taskItemStore } = useStore();
    const { loadTaskItems, taskItemRegistry, pagination, loadingInitial } = taskItemStore;
    const [currentPage, setCurrentPage] = useState(1);
    const pageSize = 10;

    useEffect(() => {
        console.log('TaskItemDashboard: useEffect triggered, loading items...');
        loadTaskItems(currentPage, pageSize).catch(error => {
            console.error('TaskItemDashboard: Error loading items:', error);
        });
    }, [currentPage, pageSize]) // Removed loadTaskItems from dependencies to avoid infinite loops

    const handlePageChange = (pageNumber: number) => {
        setCurrentPage(pageNumber);
        window.scrollTo({ top: 0, behavior: 'smooth' });
    };

    // Debug logging
    console.log('TaskItemDashboard render:', {
        loadingInitial,
        registrySize: taskItemRegistry.size,
        pagination: pagination,
        currentPage,
        pageSize
    });

    if (loadingInitial && taskItemRegistry.size === 0) {
        console.log('TaskItemDashboard: Showing loading component');
        return (<LoadingComponent content='Loading app...' />)
    }

    // Debug: log pagination data
    console.log('TaskItemDashboard pagination:', pagination);

    console.log('TaskItemDashboard: Rendering main content');

    return (
        <Grid>
            <Grid.Column width='16'>
                <div style={{ minHeight: '200px' }}>
                    <TaskItemList />
                </div>
                {pagination && (
                    <Segment textAlign="center" style={{ marginTop: '2em' }}>
                        <Pagination 
                            pagination={pagination} 
                            onPageChange={handlePageChange}
                            loading={taskItemStore.loadingInitial}
                        />
                    </Segment>
                )}
            </Grid.Column>
        </Grid>
    )
})