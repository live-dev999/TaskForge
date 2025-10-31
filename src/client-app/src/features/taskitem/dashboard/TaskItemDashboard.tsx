import { observer } from "mobx-react-lite";
import { Grid } from "semantic-ui-react";
import { useStore } from "../../../app/stores/store";
import TaskItemDetails from "../details/TaskItemDetails";
import TaskItemForm from "../form/TaskItemForm";
import TaskItemList from "./TaskItemList";
import { useEffect } from "react";
import LoadingComponent from "../../../app/layout/LoadingComponent";

export default observer(function TaskItemDashboard() {
    const { taskItemStore } = useStore();

    useEffect(() => {
        taskItemStore.loadTaskItems();
    }, [taskItemStore])

    if (taskItemStore.loadingInitial)
        return (<LoadingComponent content='Loading app...' />)

    return (
        <Grid>
            <Grid.Column width='10'>
                <TaskItemList/>
            </Grid.Column>
            <Grid.Column width='6'>
                <h2>
                    TaskItem Filters
                </h2>
            </Grid.Column>

        </Grid>
    )
})