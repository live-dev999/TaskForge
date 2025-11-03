import { Link } from "react-router-dom";
import { Button, Container, Header, Segment, Image } from "semantic-ui-react";

export default function HomePage() {
    return (
      <Segment inverted vertical textAlign='center' className="masthead">
            <Container text>
                <Header as='h1' inverted>
                    <Image size='massive' src='/assets/logo.jpeg' alt='logo' style={{ marginButtom: 12 }} />
                </Header>
                <Header as='h2' inverted content='Welcom to TASK FORGE' />
                <Button as={Link} to='/taskitems' size='huge' inverted>
                    Let's start!
                </Button>
            </Container>
        </Segment>
    
    )
}