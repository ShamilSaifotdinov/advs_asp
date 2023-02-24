import React from "react";
// import useQuery from "../hooks/hook.http";
import Adv from "../Adv"
import AdvContain from "../AdvContain";

class Main extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            loading: true,
            advs: []
        };
    }

    componentDidMount() {
        // const [ http, loading, error ] = useQuery()
        fetch(`http://${window.location.hostname}:8080/advs`)
            .then(res => res.json())
            .then(
                (result) => {
                    this.setState({
                        loading: false,
                        advs: result
                    });
                },
                // Note: it's important to handle errors here
                // instead of a catch() block so that we don't swallow
                // exceptions from actual bugs in components.
                (error) => {
                    this.setState({
                        loading: false,
                        advs: []
                    })
                    alert(error);
                    console.error(this.state.error);
                }
            )
    }

    render() {
        return (
            <div className="main">
                <div className="wrapper">
                    <h1>Объявления</h1>
                    {
                        this.state.loading
                            ? <h2>Loading...</h2>
                            : this.state.advs.map(adv => 
                                <Adv key={adv.advId} > 
                                    <AdvContain {...adv} />
                                </Adv>)
                    }
                </div>
            </div>
        );
    }
}

export default Main;
