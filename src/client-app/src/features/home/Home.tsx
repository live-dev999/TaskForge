import { Link } from "react-router-dom";
import { Button, Container, Header, Segment, Image, Icon, Divider } from "semantic-ui-react";
import './Home.css';

export default function HomePage() {
    return (
        <Segment 
            inverted 
            vertical 
            textAlign='center' 
            className="masthead home-segment"
        >
            <Container text className="home-container">
                <div className="home-content">
                    <Image 
                        size='massive' 
                        src='/assets/logo.jpeg' 
                        alt='logo'
                        className="home-logo"
                    />
                    <Header 
                        as='h1' 
                        inverted 
                        className="home-title"
                    >
                        Welcome to Task Forge
                    </Header>
                    <Header 
                        as='h2' 
                        inverted 
                        className="home-subtitle"
                    >
                        Your Ultimate Task Management Solution
                    </Header>
                    <Divider inverted className="home-divider" />
                    <div className="home-buttons">
                        <Button 
                            as={Link} 
                            to='/taskItems' 
                            size='huge' 
                            color='teal'
                            className="home-button"
                        >
                            <Icon name='tasks' />
                            Get Started
                        </Button>
                        <Button 
                            as={Link} 
                            to='/createTaskItem' 
                            size='huge' 
                            inverted
                            className="home-button-inverted"
                        >
                            <Icon name='plus circle' />
                            Create Task
                        </Button>
                    </div>
                    <div className="home-features">
                        <div className="home-feature-item">
                            <Icon name='check circle' size='big' className="home-feature-icon" />
                            <div className="home-feature-text">Organize Tasks</div>
                        </div>
                        <div className="home-feature-item">
                            <Icon name='clock' size='big' className="home-feature-icon" />
                            <div className="home-feature-text">Track Progress</div>
                        </div>
                        <div className="home-feature-item">
                            <Icon name='chart line' size='big' className="home-feature-icon" />
                            <div className="home-feature-text">Stay Productive</div>
                        </div>
                    </div>
                </div>
            </Container>
        </Segment>
    )
}