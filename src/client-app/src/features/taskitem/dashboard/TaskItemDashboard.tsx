import { observer } from "mobx-react-lite";
import { Grid, Segment } from "semantic-ui-react";
import { useStore } from "../../../app/stores/store";
import TaskItemList from "./TaskItemList";
import { useEffect, useState } from "react";
import LoadingComponent from "../../../app/layout/LoadingComponent";
import Pagination from "../../../app/components/Pagination";

export default observer(function TaskItemDashboard() {
    const { taskItemStore } = useStore();
    const { loadTaskItems, taskItemRegistry, pagination } = taskItemStore;
    const [currentPage, setCurrentPage] = useState(1);
    const pageSize = 10;

    useEffect(() => {
        loadTaskItems(currentPage, pageSize);
    }, [loadTaskItems, currentPage])

    const handlePageChange = (pageNumber: number) => {
        setCurrentPage(pageNumber);
        window.scrollTo({ top: 0, behavior: 'smooth' });
    };

    if (taskItemStore.loadingInitial && taskItemRegistry.size === 0)
        return (<LoadingComponent content='Loading app...' />)

    return (
        <Grid>
            <Grid.Column width='16'>
                <TaskItemList />
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