using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HouseAssigner : MonoBehaviour
{
    public PodContentType houseType;

    // Start is called before the first frame update
    void Start()
    {
        Managers.Planet.onPlanetStateChanged += assignUnoccupiedHouses;
    }

    void assignUnoccupiedHouses(Planet planet)
    {
        List<PodContent> houses = planet.Pods(Managers.Constants.spacePodType)
            .ConvertAll(pod => pod.getContent(houseType));
        houses.RemoveAll(house => house == null);
        houses.RemoveAll(house => Managers.Planet.Planet.residents.Any(
            stncrn => stncrn.homePosition == house.container.worldPos
            ));
        houses.ForEach(
            house =>
            {
                Stonicorn resident = Managers.Planet.Planet.residents.FirstOrDefault(
                    stncrn => Managers.Planet.Planet.getPod(stncrn.homePosition).podType
                        == Managers.Constants.corePodType
                    );
                if (resident != null)
                {
                    resident.homePosition = house.container.worldPos;
                }
            }
            );
    }
}
