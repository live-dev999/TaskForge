import { RouteObject, createBrowserRouter } from "react-router-dom";
import App from "../layout/App";
import HomePage from "../../features/home/Home";
import TaskItemDashboard from "../../features/taskitem/dashboard/TaskItemDashboard";
import TaskItemForm from "../../features/taskitem/form/TaskItemForm";


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
            path: 'taskItem',
            element: <TaskItemDashboard />
        }, {
            path: 'createTaskItem',
            element: <TaskItemForm />
        }
    ]
}]
export const router  = createBrowserRouter(routes)