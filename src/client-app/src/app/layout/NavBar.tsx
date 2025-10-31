import { Button, Container, Menu } from "semantic-ui-react";
import { NavLink } from "react-router-dom";
import { useStore } from "../stores/store";


export default function NavBar() {
    return (
        <Menu inverted fixed="top">
            <Container>
                <Menu.Item as={NavLink} to='/' header>
                    <img src="./assets/logo.jpeg" alt="logo" style={{marginRight: '10px'}}/>
                    Task Forge
               </Menu.Item>
                <Menu.Item as={NavLink} to='/taskItems' name="TaskItems">
                </Menu.Item>
                <Menu.Item name="TaskItems">
                    <Button positive content='Create TaskItems' as={NavLink} to='/createTaskItem'/>
                </Menu.Item>
            </Container>
        </Menu>
    )
}