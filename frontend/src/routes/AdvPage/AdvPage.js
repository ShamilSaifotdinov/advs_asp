import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";

export default function AdvPage() {
    const params = useParams();

    const [error, setError] = useState(null);
    const [loading, setLoading] = useState(true);
    const [adv, setAdv] = useState({});

    // Note: the empty deps array [] means
    // this useEffect will run once
    // similar to componentDidMount()
    useEffect(() => {
        fetch(`http://${window.location.hostname}:8080/advs/${params.id}`)
            .then(res => res.json())
            .then(
                (result) => {
                    setLoading(false);
                    setAdv(result);
                },
                // Note: it's important to handle errors here
                // instead of a catch() block so that we don't swallow
                // exceptions from actual bugs in components.
                (error) => {
                    setLoading(false);
                    setError(error);
                }
            )
    }, [])

    return (
        <div className="AdvPage">
            <div className="wrapper">
                {
                    loading
                    ? <h1>Loading...</h1>
                    : error
                        ? console.error(error)
                        :   <div className="AdvPage_Disc">
                                <h1>{adv.Name}</h1>
                                <p>{adv.Location}</p>
                                <p>{adv.Discription}</p>
                                <h2>{adv.Price.toFixed(2)} руб.</h2>
                            </div>
                }
            </div>
        </div>

    )
}