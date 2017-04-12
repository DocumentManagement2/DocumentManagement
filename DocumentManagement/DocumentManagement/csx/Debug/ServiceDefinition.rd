<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="DocumentManagement" generation="1" functional="0" release="0" Id="459c9cb6-ccfa-43c2-ba16-10289ac41ab9" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="DocumentManagementGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="documentManagementAdminWeb:Endpoint1" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/DocumentManagement/DocumentManagementGroup/LB:documentManagementAdminWeb:Endpoint1" />
          </inToChannel>
        </inPort>
        <inPort name="DoucmentManagementWeb:Endpoint1" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/DocumentManagement/DocumentManagementGroup/LB:DoucmentManagementWeb:Endpoint1" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="documentManagementAdminWeb:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/DocumentManagement/DocumentManagementGroup/MapdocumentManagementAdminWeb:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="documentManagementAdminWebInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/DocumentManagement/DocumentManagementGroup/MapdocumentManagementAdminWebInstances" />
          </maps>
        </aCS>
        <aCS name="DoucmentManagementWeb:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/DocumentManagement/DocumentManagementGroup/MapDoucmentManagementWeb:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="DoucmentManagementWebInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/DocumentManagement/DocumentManagementGroup/MapDoucmentManagementWebInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:documentManagementAdminWeb:Endpoint1">
          <toPorts>
            <inPortMoniker name="/DocumentManagement/DocumentManagementGroup/documentManagementAdminWeb/Endpoint1" />
          </toPorts>
        </lBChannel>
        <lBChannel name="LB:DoucmentManagementWeb:Endpoint1">
          <toPorts>
            <inPortMoniker name="/DocumentManagement/DocumentManagementGroup/DoucmentManagementWeb/Endpoint1" />
          </toPorts>
        </lBChannel>
      </channels>
      <maps>
        <map name="MapdocumentManagementAdminWeb:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/DocumentManagement/DocumentManagementGroup/documentManagementAdminWeb/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapdocumentManagementAdminWebInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/DocumentManagement/DocumentManagementGroup/documentManagementAdminWebInstances" />
          </setting>
        </map>
        <map name="MapDoucmentManagementWeb:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/DocumentManagement/DocumentManagementGroup/DoucmentManagementWeb/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapDoucmentManagementWebInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/DocumentManagement/DocumentManagementGroup/DoucmentManagementWebInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="documentManagementAdminWeb" generation="1" functional="0" release="0" software="D:\VSProjects\DocumentManagement\DocumentManagement\csx\Debug\roles\documentManagementAdminWeb" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="-1" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="http" portRanges="8080" />
            </componentports>
            <settings>
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;documentManagementAdminWeb&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;documentManagementAdminWeb&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;DoucmentManagementWeb&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/DocumentManagement/DocumentManagementGroup/documentManagementAdminWebInstances" />
            <sCSPolicyUpdateDomainMoniker name="/DocumentManagement/DocumentManagementGroup/documentManagementAdminWebUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/DocumentManagement/DocumentManagementGroup/documentManagementAdminWebFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
        <groupHascomponents>
          <role name="DoucmentManagementWeb" generation="1" functional="0" release="0" software="D:\VSProjects\DocumentManagement\DocumentManagement\csx\Debug\roles\DoucmentManagementWeb" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="-1" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="http" portRanges="80" />
            </componentports>
            <settings>
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;DoucmentManagementWeb&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;documentManagementAdminWeb&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;DoucmentManagementWeb&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/DocumentManagement/DocumentManagementGroup/DoucmentManagementWebInstances" />
            <sCSPolicyUpdateDomainMoniker name="/DocumentManagement/DocumentManagementGroup/DoucmentManagementWebUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/DocumentManagement/DocumentManagementGroup/DoucmentManagementWebFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyUpdateDomain name="DoucmentManagementWebUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyUpdateDomain name="documentManagementAdminWebUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyFaultDomain name="documentManagementAdminWebFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyFaultDomain name="DoucmentManagementWebFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="documentManagementAdminWebInstances" defaultPolicy="[1,1,1]" />
        <sCSPolicyID name="DoucmentManagementWebInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="d2929647-3b01-4405-99ea-d32a23dda897" ref="Microsoft.RedDog.Contract\ServiceContract\DocumentManagementContract@ServiceDefinition">
      <interfacereferences>
        <interfaceReference Id="2fbba5d9-8602-4145-ac3e-b1e21ae1398b" ref="Microsoft.RedDog.Contract\Interface\documentManagementAdminWeb:Endpoint1@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/DocumentManagement/DocumentManagementGroup/documentManagementAdminWeb:Endpoint1" />
          </inPort>
        </interfaceReference>
        <interfaceReference Id="af7c2feb-241c-489b-a64e-5a44661279cf" ref="Microsoft.RedDog.Contract\Interface\DoucmentManagementWeb:Endpoint1@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/DocumentManagement/DocumentManagementGroup/DoucmentManagementWeb:Endpoint1" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>