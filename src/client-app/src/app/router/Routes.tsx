import { RouteObject, createBrowserRouter } from "react-router-dom";
import App from "../layout/App";
import HomePage from "../../features/home/Home";
import TaskItemDashboard from "../../features/taskitem/dashboard/TaskItemDashboard";
import TaskItemForm from "../../features/taskitem/form/TaskItemForm";
import TaskItemDetails from "../../features/taskitem/details/TaskItemDetails";


export const routes: RouteObject[] = 
[{
    path: '/',
    element: <App/>,
    children: [
        {
            path: '',
            element: <HomePage/>
        },
        {
            path: 'taskItems',
            element: <TaskItemDashboard />
        },
        {
            path: '/taskItems/:id',
            element: <TaskItemDetails />
        }, 
        {
            path: 'createTaskItem',
            element: <TaskItemForm />
        },
         {
            path: 'manage/:id',
            element: <TaskItemForm />
        }
    ]
}]
export const router  = createBrowserRouter(routes)