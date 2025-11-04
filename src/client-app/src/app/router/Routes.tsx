import { Navigate, RouteObject, createBrowserRouter } from "react-router-dom";
import App from "../layout/App";
import HomePage from "../../features/home/Home";
import TaskItemDashboard from "../../features/taskitem/dashboard/TaskItemDashboard";
import TaskItemDetails from "../../features/taskitem/details/TaskItemDetails";
import TaskItemForm from "../../features/taskitem/form/TaskItemForm";
import NotFound from "../../features/errors/NotFound";
import ServerError from "../../features/errors/ServerError";
import TestErrors from "../../features/errors/TestErrors";


export const routes: RouteObject[] =
    [{
        path: '/',
        element: <App />,
        children: [
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
            },
             {
                path: 'errors',
                element: <TestErrors />
            },
            {
                path: 'not-found',
                element: <NotFound />
            },
            {
                path: 'server-error',
                element: <ServerError />
            },
            {
                path: '*',
                element: <Navigate replace to='/not-found' />
            }
        ]
    }]
export const router = createBrowserRouter(routes)