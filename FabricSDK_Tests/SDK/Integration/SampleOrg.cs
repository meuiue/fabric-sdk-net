/*
 *  Copyright 2016, 2017 DTCC, Fujitsu Australia Software Technology, IBM - All Rights Reserved.
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *      http://www.apache.org/licenses/LICENSE-2.0
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */

using System.Collections.Generic;
using System.Linq;
using Hyperledger.Fabric.SDK;
using Hyperledger.Fabric.SDK.Helper;
using Hyperledger.Fabric_CA.SDK;

namespace Hyperledger.Fabric.Tests.SDK.Integration
{
    /**
     * Sample Organization Representation
     *
     * Keeps track which resources are defined for the Organization it represents.
     *
     */
    public class SampleOrg
    {
        private readonly Dictionary<string, string> eventHubLocations = new Dictionary<string, string>();
        private readonly string mspid;
        private readonly string name;
        private readonly Dictionary<string, string> ordererLocations = new Dictionary<string, string>();
        private readonly Dictionary<string, string> peerLocations = new Dictionary<string, string>();

        private readonly Dictionary<string, IUser> userMap = new Dictionary<string, IUser>();
        private SampleUser admin;
        private HFCAClient caClient;
        private string caLocation;

        private string caName;
        private Properties caProperties = null;


        private string domainName;

        private SampleUser peerAdmin;

        public SampleOrg(string name, string mspid)
        {
            this.name = name;
            this.mspid = mspid;
        }

        public string getCAName()
        {
            return caName;
        }

        public SampleUser GetAdmin()
        {
            return admin;
        }

        public void SetAdmin(SampleUser admin)
        {
            this.admin = admin;
        }

        public string GetMSPID()
        {
            return mspid;
        }

        public string GetCALocation()
        {
            return caLocation;
        }

        public void SetCALocation(string caLocation)
        {
            this.caLocation = caLocation;
        }

        public void AddPeerLocation(string name, string location)
        {
            peerLocations.Add(name, location);
        }

        public void AddOrdererLocation(string name, string location)
        {
            ordererLocations.Add(name, location);
        }

        public void AddEventHubLocation(string name, string location)
        {
            eventHubLocations.Add(name, location);
        }

        public string GetPeerLocation(string name)
        {
            return peerLocations.GetOrNull(name);
        }

        public string GetOrdererLocation(string name)
        {
            return ordererLocations.GetOrNull(name);
        }

        public string GetEventHubLocation(string name)
        {
            return eventHubLocations.GetOrNull(name);
        }

        public IReadOnlyList<string> GetPeerNames()
        {
            return peerLocations.Keys.ToList();
        }


        public IReadOnlyList<string> GetOrdererNames()
        {
            return ordererLocations.Keys.ToList();
        }

        public IReadOnlyList<string> GetEventHubNames()
        {
            return eventHubLocations.Keys.ToList();
        }

        public HFCAClient GetCAClient()
        {
            return caClient;
        }

        public void SetCAClient(HFCAClient caClient)
        {
            this.caClient = caClient;
        }

        public string GetName()
        {
            return name;
        }

        public void AddUser(SampleUser user)
        {
            userMap.Add(user.Name, user);
        }

        public IUser GetUser(string name)
        {
            return userMap.GetOrNull(name);
        }

        public IReadOnlyList<string> getOrdererLocations()
        {
            return ordererLocations.Values.ToList();
        }

        public IReadOnlyList<string> getEventHubLocations()
        {
            return eventHubLocations.Values.ToList();
        }


        public void SetCAProperties(Properties caProperties)
        {
            this.caProperties = caProperties;
        }

        public Properties GetCAProperties()
        {
            return caProperties;
        }


        public SampleUser GetPeerAdmin()
        {
            return peerAdmin;
        }

        public void SetPeerAdmin(SampleUser peerAdmin)
        {
            this.peerAdmin = peerAdmin;
        }

        public void SetDomainName(string domainName)
        {
            this.domainName = domainName;
        }

        public string GetDomainName()
        {
            return domainName;
        }

        public void SetCAName(string caName)
        {
            this.caName = caName;
        }
    }
}