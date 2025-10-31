import React from "react";
import { Button, Container, Menu } from "semantic-ui-react";
import { useStore } from "../stores/store";

interface Props {
    openForm: () => void;
}

export default function NavBar() {
     const { taskItemStore } = useStore()
    return (
        <Menu inverted fixed="top">
            <Container>
                <Menu.Item header>
                    <img src="./assets/logo.jpeg" alt="logo" style={{marginRight: '10px'}}/>
                    Task Forge
                </Menu.Item>
                <Menu.Item name="TaskItems">
                </Menu.Item>
                <Menu.Item name="TaskItems">
                    <Button positive content='Create TaskItem' onClick={() => taskItemStore.openForm()}/>
                </Menu.Item>
            </Container>
        </Menu>
    )
}