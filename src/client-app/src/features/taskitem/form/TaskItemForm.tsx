import { useEffect } from "react";
import { Button, Form, Segment, Label } from "semantic-ui-react";
import { TaskItem, TaskStatus } from "../../../app/models/taskItem";
import { observer } from "mobx-react-lite";
import { Link, useNavigate, useParams } from "react-router-dom";
import LoadingComponent from "../../../app/layout/LoadingComponent";
import { useStore } from "../../../app/stores/store";
import { Formik, FormikHelpers, FormikProps } from "formik";
import * as Yup from "yup";
import { toast } from "react-toastify";

interface TaskItemFormValues {
    id: string;
    title: string;
    description: string;
    createdAt: string;
    updatedAt: string;
    status: TaskStatus;
}

const validationSchema = Yup.object({
    title: Yup.string()
        .required('Title is required')
        .max(500, 'Title must be 500 characters or less')
        .min(3, 'Title must be at least 3 characters'),
    description: Yup.string()
        .max(2000, 'Description must be 2000 characters or less'),
    createdAt: Yup.string()
        .required('Created date is required'),
    updatedAt: Yup.string()
        .required('Updated date is required'),
    status: Yup.number()
        .required('Status is required')
        .oneOf([TaskStatus.New, TaskStatus.InProgress, TaskStatus.Completed, TaskStatus.Pending], 'Invalid status')
});

export default observer(function TaskItemForm() {
    const { taskItemStore } = useStore();
    const { createTaskItem, updateTaskItem, loading, loadTaskItem, loadingInitial, selectedTaskItem } = taskItemStore;
    const { id } = useParams();
    const navigate = useNavigate();

    useEffect(() => {
        if (id) {
            loadTaskItem(id).then(taskItem => {
                if (!taskItem) {
                    // Task item not found, navigation should be handled by interceptor
                    console.warn('Task item not found, redirecting...');
                }
            }).catch(error => {
                console.error('Failed to load task item:', error);
                // Error is already handled by axios interceptor
            });
        }
    }, [id, loadTaskItem]);

    const getInitialValues = (): TaskItemFormValues => {
        if (id && selectedTaskItem && selectedTaskItem.id === id) {
            return {
                id: selectedTaskItem.id,
                title: selectedTaskItem.title,
                description: selectedTaskItem.description || '',
                createdAt: selectedTaskItem.createdAt,
                updatedAt: selectedTaskItem.updatedAt,
                status: selectedTaskItem.status
            };
        }
        // Default values for new task
        const today = new Date().toISOString().split('T')[0];
        return {
            id: '',
            title: '',
            description: '',
            createdAt: today,
            updatedAt: today,
            status: TaskStatus.New
        };
    };

    const initialValues = getInitialValues();

    const statusOptions = Object.entries(TaskStatus)
        .filter(([key, value]) => isNaN(Number(key)))
        .map(([key, value]) => ({
            key: key,
            text: key,
            value: value
        }));

    if (loadingInitial) return <LoadingComponent content="Loading taskItem ..." />

    return (
        <Segment clearing>
            <Formik
                initialValues={initialValues}
                validationSchema={validationSchema}
                enableReinitialize
                onSubmit={async (values: TaskItemFormValues, { setSubmitting }: FormikHelpers<TaskItemFormValues>) => {
                    try {
                        if (id) {
                            // Editing existing task
                            const taskItemToSubmit: TaskItem = {
                                id: id,
                                title: values.title,
                                description: values.description || '',
                                status: values.status,
                                createdAt: values.createdAt,
                                updatedAt: values.updatedAt
                            };
                            await updateTaskItem(taskItemToSubmit);
                            navigate(`/taskItems/${id}`);
                        } else {
                            // Creating new task - send Guid.Empty as string, server will generate new Guid
                            // Server will set CreatedAt and UpdatedAt automatically, but we can send current dates
                            const taskItemToSubmit: TaskItem = {
                                id: '00000000-0000-0000-0000-000000000000', // Guid.Empty for new task
                                title: values.title,
                                description: values.description || '',
                                status: values.status,
                                createdAt: values.createdAt, // Server will overwrite this
                                updatedAt: values.updatedAt  // Server will overwrite this
                            };
                            const createdTaskItem = await createTaskItem(taskItemToSubmit);
                            if (createdTaskItem && createdTaskItem.id) {
                                navigate(`/taskItems/${createdTaskItem.id}`);
                            } else {
                                console.error('Failed to create task item: no ID returned');
                                toast.error('Failed to create task item');
                            }
                        }
                    } catch (error) {
                        console.error('Error submitting form:', error);
                        // Error is already handled by agent interceptor
                    } finally {
                        setSubmitting(false);
                    }
                }}
            >
                {({ values, errors, touched, handleChange, handleBlur, handleSubmit, setFieldValue, isSubmitting }: FormikProps<TaskItemFormValues>) => (
                    <Form onSubmit={handleSubmit}>
                        <Form.Field>
                            <Form.Input
                                placeholder='Title'
                                value={values.title}
                                name='title'
                                onChange={handleChange}
                                onBlur={handleBlur}
                                error={errors.title && touched.title ? { content: errors.title, pointing: 'below' } : null}
                            />
                        </Form.Field>

                        <Form.Field>
                            <Form.TextArea
                                placeholder='Description'
                                value={values.description}
                                name='description'
                                onChange={handleChange}
                                onBlur={handleBlur}
                                error={errors.description && touched.description ? { content: errors.description, pointing: 'below' } : null}
                            />
                        </Form.Field>

                        <Form.Field>
                            <label>Created Date</label>
                            <Form.Input
                                type='date'
                                placeholder='Create Date'
                                value={values.createdAt}
                                name='createdAt'
                                onChange={handleChange}
                                onBlur={handleBlur}
                                error={errors.createdAt && touched.createdAt ? { content: errors.createdAt, pointing: 'below' } : null}
                            />
                        </Form.Field>

                        <Form.Field>
                            <label>Updated Date</label>
                            <Form.Input
                                type='date'
                                placeholder='Update Date'
                                value={values.updatedAt}
                                name='updatedAt'
                                onChange={handleChange}
                                onBlur={handleBlur}
                                error={errors.updatedAt && touched.updatedAt ? { content: errors.updatedAt, pointing: 'below' } : null}
                            />
                        </Form.Field>

                        <Form.Field
                            error={errors.status && touched.status ? { content: errors.status } : null}
                        >
                            <label>Task Status</label>
                            <Form.Select
                                placeholder="Select Status"
                                options={statusOptions}
                                value={values.status}
                                onChange={(e, data) => {
                                    setFieldValue('status', data.value);
                                }}
                                onBlur={() => handleBlur({ target: { name: 'status' } } as any)}
                                name="status"
                            />
                            {errors.status && touched.status && (
                                <Label basic color='red' pointing>
                                    {errors.status}
                                </Label>
                            )}
                        </Form.Field>

                        <Button
                            loading={loading || isSubmitting}
                            floated='right'
                            positive
                            type='submit'
                            content='Submit'
                            disabled={isSubmitting}
                        />
                        <Button
                            floated='right'
                            as={Link}
                            to={'/taskItems'}
                            type='button'
                            content='Cancel'
                        />
                    </Form>
                )}
            </Formik>
        </Segment>
    )
})

