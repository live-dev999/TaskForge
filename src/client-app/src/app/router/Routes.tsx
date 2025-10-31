import { RouteObject, createBrowserRouter } from "react-router-dom";
import App from "../layout/App";
import HomePage from "../../features/home/Home";
import TaskItemDashboard from "../../features/taskitem/dashboard/TaskItemDashboard";
import TaskItemDetails from "../../features/taskitem/details/TaskItemDetails";
import TaskItemForm from "../../features/taskitem/form/TaskItemForm";


export const routes: RouteObject[] =
    [{
        path: '/',
        element: <App />,
        children: [
            {
                path: '',
                element: <HomePage />
            },
            {
                path: '/taskItems',
                element: <TaskItemDashboard />
            },
            {
                path: '/taskItems/:id',
                element: <TaskItemDetails />
            },
            {
                path: 'createTaskItem',
                element: <TaskItemForm key='create' />
            },
            {
                path: 'manage/:id',
                element: <TaskItemForm key='manage' />
            }
        ]
    }]
export const router = createBrowserRouter(routes)