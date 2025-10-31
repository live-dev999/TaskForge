import { observer } from "mobx-react-lite";
import { Grid } from "semantic-ui-react";
import { useStore } from "../../../app/stores/store";
import TaskItemDetails from "../details/TaskItemDetails";
import TaskItemForm from "../form/TaskItemForm";
import TaskItemList from "./TaskItemList";

export default observer(function TaskItemDashboard() {

    const { taskItemStore } = useStore()
    const { selectedTaskItem, editMode } = taskItemStore;
    return (
        <Grid>
            <Grid.Column width='10'>
                <TaskItemList/>
            </Grid.Column>
            <Grid.Column width='6'>
                {selectedTaskItem && !editMode &&
                    <TaskItemDetails />}
                {editMode &&
                    <TaskItemForm
                    />}
            </Grid.Column>

        </Grid>
    )
})