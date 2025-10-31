import { Link } from "react-router-dom";
import { Container } from "semantic-ui-react";

export default function HomePage(){
    return(
        <Container>
            <h1>Home page</h1>

            <h3>go to <Link to='/taskItems'>TaskItems</Link></h3>
        </Container>
    )
}