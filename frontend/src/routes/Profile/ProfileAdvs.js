import { useEffect, useState } from "react"
// import useQuery from "../../hooks/hook.http";
import Adv from "../Adv"
import AdvContain from "../AdvContain";
import AdvMenu from "./AdvMenu";

export default function ProfileAdvs() {
    /*
    const http = useQuery()
    
    useEffect(() => {
            if (errorState) {
                console.error(errorState)
                alert(errorState)
            }
        }, [errorState])
    
    useEffect(() => {
            var res;
            async function fetchData() {
                res = await http(
                    "/profile", 
                    'GET',
                    {}, 
                    {
                        "Accept": "application/json",
                        "Authorization": "Bearer " + localStorage.getItem('token')  // передача токена в заголовке
                    }
                )
            }

            fetchData();
            
            if (res) {
                console.log(res);
                setAdvs(res);
            }
        }, [])
        */
    const [ advs, setAdvs ] = useState({});
    const [error, setError] = useState(null);
    const [loading, setLoading] = useState(true);
    
    useEffect(() => {
        fetch(`http://${window.location.hostname}:8080/profile`,
            {
                method: "GET",
                headers: {
                    "Accept": "application/json",
                    "Authorization": "Bearer " + localStorage.getItem('token')  // передача токена в заголовке
                }
            })
            .then(res => {
                if (res.status === 401) {
                    localStorage.clear()
                    throw new Error("Необходимо авторизоваться!")
                }
                return res.json()
            })
            .then(
                (result) => {
                    setLoading(false);
                    setAdvs(result);
                },
                // Note: it's important to handle errors here
                // instead of a catch() block so that we don't swallow
                // exceptions from actual bugs in components.
                (error) => {
                    setLoading(false);
                    setError(error);
                    alert(error);
                }
            )
    }, [loading])
    
    return (
        <div className="ProfileAdvs">
            {
                loading
                    ? <h1>Loading...</h1>
                    : error
                        ? console.error(error)
                        : advs.map(adv => 
                            <Adv key={adv.advId}>
                                <AdvContain {...adv} />
                                <AdvMenu advId={adv.advId} updateParent={() => setLoading(true)} />
                            </Adv>)
            }
        </div>

    )
}