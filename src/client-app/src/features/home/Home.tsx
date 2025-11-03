import { Link } from "react-router-dom";
import { Button, Container, Header, Segment, Image, Icon, Divider } from "semantic-ui-react";

export default function HomePage() {
    return (
        <Segment 
            inverted 
            vertical 
            textAlign='center' 
            className="masthead"
            style={{
                minHeight: '100vh',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
                padding: '0'
            }}
        >
            <Container text style={{ width: '100%', maxWidth: '800px' }}>
                <div style={{ 
                    display: 'flex', 
                    flexDirection: 'column', 
                    alignItems: 'center', 
                    justifyContent: 'center',
                    minHeight: '100vh',
                    padding: '2em 0'
                }}>
                    <Image 
                        size='massive' 
                        src='/assets/logo.jpeg' 
                        alt='logo' 
                        style={{ 
                            marginBottom: '2em',
                            borderRadius: '50%',
                            boxShadow: '0 8px 16px rgba(0,0,0,0.3)',
                            maxWidth: '200px',
                            height: 'auto'
                        }} 
                    />
                    <Header 
                        as='h1' 
                        inverted 
                        style={{ 
                            fontSize: '4em',
                            marginBottom: '0.5em',
                            fontWeight: 'bold',
                            textShadow: '2px 2px 4px rgba(0,0,0,0.3)'
                        }}
                    >
                        Welcome to Task Forge
                    </Header>
                    <Header 
                        as='h2' 
                        inverted 
                        style={{ 
                            fontSize: '1.8em',
                            fontWeight: 'normal',
                            marginBottom: '2em',
                            color: 'rgba(255,255,255,0.9)'
                        }}
                    >
                        Your Ultimate Task Management Solution
                    </Header>
                    <Divider inverted style={{ margin: '2em 0', width: '100px', marginLeft: 'auto', marginRight: 'auto' }} />
                    <div style={{ 
                        display: 'flex', 
                        gap: '1em', 
                        justifyContent: 'center',
                        flexWrap: 'wrap',
                        marginTop: '2em'
                    }}>
                        <Button 
                            as={Link} 
                            to='/taskItems' 
                            size='huge' 
                            color='teal'
                            style={{
                                minWidth: '200px',
                                padding: '1.2em 2em',
                                fontSize: '1.2em',
                                boxShadow: '0 4px 8px rgba(0,0,0,0.3)'
                            }}
                        >
                            <Icon name='tasks' />
                            Get Started
                        </Button>
                        <Button 
                            as={Link} 
                            to='/createTaskItem' 
                            size='huge' 
                            inverted
                            style={{
                                minWidth: '200px',
                                padding: '1.2em 2em',
                                fontSize: '1.2em',
                                border: '2px solid white',
                                boxShadow: '0 4px 8px rgba(0,0,0,0.3)'
                            }}
                        >
                            <Icon name='plus circle' />
                            Create Task
                        </Button>
                    </div>
                    <div style={{
                        marginTop: '4em',
                        display: 'flex',
                        gap: '2em',
                        justifyContent: 'center',
                        flexWrap: 'wrap',
                        color: 'rgba(255,255,255,0.8)'
                    }}>
                        <div style={{ textAlign: 'center' }}>
                            <Icon name='check circle' size='big' style={{ display: 'block', marginBottom: '0.5em' }} />
                            <div style={{ fontSize: '0.9em' }}>Organize Tasks</div>
                        </div>
                        <div style={{ textAlign: 'center' }}>
                            <Icon name='clock' size='big' style={{ display: 'block', marginBottom: '0.5em' }} />
                            <div style={{ fontSize: '0.9em' }}>Track Progress</div>
                        </div>
                        <div style={{ textAlign: 'center' }}>
                            <Icon name='chart line' size='big' style={{ display: 'block', marginBottom: '0.5em' }} />
                            <div style={{ fontSize: '0.9em' }}>Stay Productive</div>
                        </div>
                    </div>
                </div>
            </Container>
        </Segment>
    )
}