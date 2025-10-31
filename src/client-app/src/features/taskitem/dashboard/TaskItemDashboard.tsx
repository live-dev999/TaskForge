import { observer } from "mobx-react-lite";
import { Grid } from "semantic-ui-react";
import { useStore } from "../../../app/stores/store";
import TaskItemList from "./TaskItemList";
import { useEffect } from "react";
import LoadingComponent from "../../../app/layout/LoadingComponent";

export default observer(function TaskItemDashboard() {
    const { taskItemStore } = useStore();
    const { loadTaskItems, taskItemRegistry } = taskItemStore;
    useEffect(() => {
        if (taskItemRegistry.size <= 1) loadTaskItems();
    }, [loadTaskItems,taskItemRegistry])

    if (taskItemStore.loadingInitial)
        return (<LoadingComponent content='Loading app...' />)

    return (
        <Grid>
            <Grid.Column width='10'>
                <TaskItemList />
            </Grid.Column>
            <Grid.Column width='6'>
                <h2>
                    TaskItem Filters
                </h2>
            </Grid.Column>

        </Grid>
    )
})